using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace VCBackend.ExternalServices.Ticketing
{
    public class Card4BTicketingKernelProxy
    {
        private HttpClient Client;
        private static String ServerUri = "http://localhost:81/api/tk/server";

        public Card4BTicketingKernelProxy()
        {
            Client = new HttpClient(); 
        }

        //This method should validate the validity of the request in the catalog
        public bool ApproveLoadCardRequest(LoadCard Request)
        {
            if (Request != null)
            {
                if (
                    Request.Price > 0 &&
                    Request.SaleDate.Year == DateTime.Now.Year &&
                    Request.SaleDate.DayOfYear == DateTime.Now.DayOfYear &&
                    Request.State == LoadRequest.STATE_CREATED
                    )
                {

                    Request.ProdId = "2065208";
                    Request.ApproveLoad();
                    return true;
                }
            }
            return false;
        }

        public bool ApproveLoadTokenRequest(LoadToken Request)
        {
            if (Request != null)
            {
                if (
                    Request.SaleDate.Year == DateTime.Now.Year &&
                    Request.SaleDate.DayOfYear == DateTime.Now.DayOfYear &&
                    Request.State == LoadRequest.STATE_CREATED &&
                    Request.DateInitial.HasValue &&
                    Request.DateInitial.Value.AddMinutes(10) >= DateTime.Now &&
                    Request.DateInitial.Value.Year == DateTime.Now.Year &&
                    Request.DateInitial.Value.DayOfYear == DateTime.Now.DayOfYear
                    )
                {

                    Request.ProdId = "2065209";
                    Request.Price = 100.0;
                    Request.ApproveLoad();
                    return true;
                }
            }
            return false;
        }

        public bool LoadCard(LoadCard Request)
        {
            if (Request.State == LoadRequest.STATE_APPROVED 
                && Request.Price >= 0)
            {
                string tkmsg = 
                    "<tkmsg><load>" +
                    "<product card_value=\"2065208\"><attribs>" +
                    "<attrib type=\"stored_value\">" + Request.Price + "</attrib>" +
                    "<attrib type=\"sale_device\">42</attrib>" +
                    "<attrib type=\"sale_date\">" + Request.SaleDate.ToString("yyyy-MM-dd") + "</attrib>" +
                    "<attrib type=\"sale_number_daily\">" + Request.Id + "</attrib>" +
                    "</attribs></product></load></tkmsg>";
                LoadResult result = LoadProductEx(tkmsg, Request, Request.VCard.Data).Result;
                if (Request.State == LoadRequest.STATE_SUCCESS)
                {
                    //apply write operations
                    WriteOperationsToCard(result.card_messages, Request.VCard);
                }
            }
            return false;
        }

        public bool LoadToken(LoadToken Request)
        {
            if (Request.State == LoadRequest.STATE_APPROVED)
            {
                Request.ProdId = "2065209";
                string tkmsg =
                    "<tkmsg><load>" +
                    "<product card_value=\"2065209\"><attribs>" +
                    "<attrib type=\"stored_value\">" + Request.Price + "</attrib>" +
                    "<attrib type=\"sale_device\">42</attrib>" +
                    "<attrib type=\"sale_date\">" + Request.SaleDate.ToString("yyyy-MM-dd") + "</attrib>" +
                    "<attrib type=\"sale_number_daily\">" + Request.Id + "</attrib>" +
                    "<attrib type=\"date_initial\">" + Request.DateInitial.Value.ToString("yyyy-MM-dd HH:mm:ss") + "</attrib>" +
                    "</attribs></product></load></tkmsg>";
                LoadResult result = LoadProductEx(tkmsg, Request, Request.VCardToken.Data).Result;
                if (Request.State == LoadRequest.STATE_SUCCESS)
                {
                    //apply write operations
                    WriteOperationsToCard(result.card_messages, Request.VCardToken);
                    //Parse TKMSG xml to find date_initial and date_final
                    var attribs = ParseTKMsg(result.msg);
                    return true;
                }
            }
            return false;
        }

        private async Task<LoadResult> LoadProductEx(String TKMsg, LoadRequest Request, String Data)
        {
            //Call WS to get a server url to load the card
            var getURLTask = Client.GetAsync(ServerUri);
            
            //While waiting for the response, do some work
            var card = new JObject();
            card["type"] = "CTS512B";
            card["data"] = Data;

            Regex r = new Regex(@"/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/");

            var loadReq = new JObject();
            loadReq["tkmsg"] = TKMsg;
            loadReq["card"] = card;

            await getURLTask; //wait for call to be finnished

            //read the URL
            string strRsp = getURLTask.Result.Content.ReadAsStringAsync().Result;
            var jsonRsp = JObject.Parse(strRsp);
            string tkUri = jsonRsp["URL"].ToString();

            if (!r.IsMatch(tkUri)) return null;

            //call the loading server
            var response = Client.PostAsJsonAsync(tkUri, loadReq).Result;
            strRsp = response.Content.ReadAsStringAsync().Result;
            jsonRsp = JObject.Parse(strRsp);
            var rslt = jsonRsp.ToObject<LoadResult>();

            if (rslt.result != null)
            {
                if (rslt.result == 0)
                    Request.SuccessfullLoad();
                else Request.FailedLoad((int)rslt.result);
            }
            else Request.FailedLoad();

            if (Request.State == LoadRequest.STATE_FAILED)
                rslt.card_messages.Clear(); // just to make sure, no write messages are returned!

            return rslt;
        }

        private void WriteOperationsToCard(IList<VCardWriteOperation> Operations, VCard Card)
        {
            foreach (VCardWriteOperation oper in Operations)
            {
                Card.Write(
                    oper.Address,
                    Convert.FromBase64String(oper.Data),
                    oper.Len
                    );
            }
        }

        private void WriteOperationsToCard(IList<VCardWriteOperation> Operations, VCardToken Card)
        {
            foreach (VCardWriteOperation oper in Operations)
            {
                Card.Write(
                    oper.Address,
                    Convert.FromBase64String(oper.Data),
                    oper.Len
                    );
            }
        }

        private object ParseTKMsg(String TKMsg)
        {
            int date_initial = 0, date_final = 0, stored_value = 0;

            using (XmlReader reader = XmlReader.Create(new StringReader(TKMsg)))
            {
                reader.ReadToFollowing("products");
                string products = reader.ReadInnerXml();

                using (XmlReader prodReader = XmlReader.Create(new StringReader(products)))
                {
                    while (prodReader.Read())
                    {
                        if (prodReader.Name.Equals("attrib") && (prodReader.NodeType == XmlNodeType.Element))
                        {
                            prodReader.MoveToFirstAttribute();
                            if (prodReader.Name.Equals("type"))
                            {
                                switch (prodReader.Value)
                                {
                                    case "date_initial":
                                        prodReader.MoveToElement();
                                        date_initial = prodReader.ReadElementContentAsInt();
                                        break;
                                    case "date_final":
                                        prodReader.MoveToElement();
                                        date_final = prodReader.ReadElementContentAsInt();
                                        break;
                                    case "stored_value":
                                        prodReader.MoveToElement();
                                        stored_value = prodReader.ReadElementContentAsInt();
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return new { date_initial = date_initial, date_final = date_final, stored_value = stored_value };
        }
    }

    public class VCardWriteOperation
    {
        public uint Address { get; set; }
        public uint Len { get; set; }
        public String Data { get; set; }

        public VCardWriteOperation() { }
    }

    public class LoadResult
    {
        public uint result { get; set; }
        public string msg { get; set; }
        public IList<VCardWriteOperation> card_messages { get; set; }

        public LoadResult()
        {
            card_messages = new List<VCardWriteOperation>();
        }
    }
}