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
using VCBackend.Exceptions;

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
        public bool ApproveLoadCardRequest(LoadRequest Request)
        {
            if (Request != null)
            {
                if (Request.VCard == null) throw new InvalidLoadRequest(String.Format("Load Card Request must have a VCard associated!"));
                if (Request.Price <= 5.0 && Request.Price >= 50.0) throw new InvalidLoadRequest(String.Format("Ammount of {0} is not valid.", Request.Price));
                if (Request.SaleDate.Date != DateTime.Today.Date) throw new InvalidLoadRequest("Internal error: sale date must be today");
                if (Request.State != LoadRequest.STATE_CREATED) throw new InvalidLoadRequest("Internal error: request not valid!");
                Request.ProdId = "2065208";
                Request.ApproveLoad();
                return true;
            }
            return false;
        }

        public bool ApproveLoadTokenRequest(LoadRequest Request)
        {
            if (Request != null)
            {
                if (Request.VCardToken == null) throw new InvalidLoadRequest("Load Token Request must have a VCardToken associated!");
                if (Request.SaleDate.Date != DateTime.Today.Date) throw new InvalidLoadRequest("Internal error: sale date must be today");
                if (Request.State != LoadRequest.STATE_CREATED) throw new InvalidLoadRequest("Internal error: request not valid!");
                if (!Request.DateInitial.HasValue || Request.DateInitial.Value.AddMinutes(10).Date < DateTime.Today.Date)
                    throw new InvalidLoadRequest("Initial token date is not valid!");

                Request.ProdId = "2065209";
                Request.Price = 1.0;
                Request.ApproveLoad();
                return true;
            }
            return false;
        }

        public double ReadCardBalance(VCard Card)
        {
            string tkmsg = "<tkmsg><read /></tkmsg>";
            TKResult result = TKCommandEx(tkmsg, Card.Data);
            if (result != null &&
                result.status == (uint)TicketingKernel.Status.READ &&
                result.result == (uint)TicketingKernel.Result.OK)
            {
                ProductAttribs attribs = ParseTKMsgNotifyComplete(result.msg);
                return attribs.stored_value;
            }
            return -1;
        }

        public bool LoadCard(LoadRequest Request)
        {
            if (Request.State == LoadRequest.STATE_APPROVED
                && Request.Price >= 0)
            {
                string tkmsg =
                    "<tkmsg><load>" +
                    "<product card_value=\"2065208\"><attribs>" +
                    "<attrib type=\"stored_value\">" + (int)Request.Price * 100 + "</attrib>" +
                    "<attrib type=\"sale_device\">42</attrib>" +
                    "<attrib type=\"sale_date\">" + Request.SaleDate.ToString("yyyy-MM-dd") + "</attrib>" +
                    "<attrib type=\"sale_number_daily\">" + Request.Id + "</attrib>" +
                    "</attribs></product></load></tkmsg>";
                TKResult result = TKCommandEx(tkmsg, Request.VCard.Data);
                if (result != null)
                {
                    if (result.status == (uint)TicketingKernel.Status.LOAD &&
                        result.result == (uint)TicketingKernel.Result.OK)
                    {
                        Request.SuccessfullLoad();
                        //apply write operations
                        WriteOperationsToCard(result.card_messages, Request.VCard);
                        //Parse TKMSG xml to find date_initial and date_final
                        var attribs = ParseTKMsgNotifyComplete(result.msg);
                        Request.ResultantBalance = attribs.stored_value;
                        return true;
                    }
                }
                Request.FailedLoad((int)result.result);
            }
            return false;
        }

        public bool LoadToken(LoadRequest Request)
        {
            if (Request.State == LoadRequest.STATE_APPROVED)
            {
                Request.ProdId = "2065209";
                string tkmsg =
                    "<tkmsg><load>" +
                    "<product card_value=\"2065209\"><attribs>" +
                    "<attrib type=\"stored_value\">" + (int)Request.Price * 100 + "</attrib>" +
                    "<attrib type=\"sale_device\">42</attrib>" +
                    "<attrib type=\"sale_date\">" + Request.SaleDate.ToString("yyyy-MM-dd") + "</attrib>" +
                    "<attrib type=\"sale_number_daily\">" + Request.Id + "</attrib>" +
                    "<attrib type=\"date_initial\">" + Request.DateInitial.Value.ToString("yyyy-MM-dd HH:mm:ss") + "</attrib>" +
                    "</attribs></product></load></tkmsg>";
                TKResult result = TKCommandEx(tkmsg, Request.VCardToken.Data);
                if (result != null)
                {
                    if (result.status == (uint)TicketingKernel.Status.LOAD &&
                        result.result == (uint)TicketingKernel.Result.OK)
                    {
                        Request.SuccessfullLoad();
                        //apply write operations
                        WriteOperationsToCard(result.card_messages, Request.VCardToken);
                        //Parse TKMSG xml to find date_initial and date_final
                        var attribs = ParseTKMsgNotifyComplete(result.msg);
                        Request.VCardToken.DateInitial = attribs.date_initial;
                        Request.DateInitial = attribs.date_initial;
                        Request.VCardToken.DateFinal = attribs.date_final;
                        Request.DateFinal = attribs.date_final;
                        Request.VCardToken.Amount = attribs.stored_value;
                        Request.ResultantBalance = attribs.stored_value;
                        return true;
                    }
                }
                Request.FailedLoad((int)result.result);
            }
            return false;
        }

        public TKTransactionData GetTransactionData(String transaction)
        {
            TKTransactionData trx = new TKTransactionData();

            String tkmsg_in =
                "<tkmsg><decode><transaction>" +
                transaction +
                "</transaction></decode></tkmsg>";
            TKResult result = TKCommandEx(tkmsg_in, null);
            if (result != null)
            {
                if (result.result == (uint)TicketingKernel.Result.OK)
                {
                    trx = ParseTKDecodeResult(result.msg);
                }
                else return null;
            }
            return trx;
        }

        private TKResult TKCommandEx(String TKMsg, String CardData)
        {
            //Call WS to get a server url to load the card
            var getURLTask = Client.GetAsync(ServerUri);

            //While waiting for the response, do some work
            var card = new JObject();
            card["type"] = "CTS512B";
            card["data"] = CardData;

            Regex r = new Regex(@"(http?:\/\/)?([\da-z\.-]+)(\.([a-z\.]{2,6}))?(\:[0-9]{2,6})?([\/\w \.-]*)*\/?");

            var loadReq = new JObject();
            loadReq["tkmsg"] = TKMsg;
            loadReq["card"] = card;

            //read the URL
            string strRsp = getURLTask.Result.Content.ReadAsStringAsync().Result;
            var jsonRsp = JObject.Parse(strRsp);
            string tkUri = jsonRsp["URL"].ToString();

            if (!r.IsMatch(tkUri)) return null;

            //call the loading server
            var loadTask = Client.PostAsJsonAsync(tkUri, loadReq);

            strRsp = loadTask.Result.Content.ReadAsStringAsync().Result;
            jsonRsp = JObject.Parse(strRsp);

            var rslt = jsonRsp.ToObject<TKResult>();
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

        private ProductAttribs ParseTKMsgNotifyComplete(String TKMsg)
        {
            ProductAttribs attribs = new ProductAttribs();

            using (XmlReader reader = XmlReader.Create(new StringReader(TKMsg)))
            {
                reader.ReadToFollowing("products");
                string products = reader.ReadInnerXml();


                using (XmlReader prodReader = XmlReader.Create(new StringReader(products)))
                {
                    string attrib = "";
                    while (prodReader.Read())
                    {
                        if (prodReader.NodeType == XmlNodeType.Element && prodReader.Name == "attrib")
                        {
                            attrib = prodReader.GetAttribute("type");
                            while (prodReader.Read())
                            {
                                if (prodReader.NodeType == XmlNodeType.Text)
                                {
                                    int value = Int32.Parse(prodReader.Value);
                                    switch (attrib)
                                    {
                                        case "date_initial":
                                            attribs.SetDateInitialFromSec(value);
                                            break;
                                        case "date_final":
                                            attribs.SetDateFinalFromSec(value);
                                            break;
                                        case "stored_value":
                                            attribs.SetAmountFromCents(value);
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return attribs;
        }

        private TKTransactionData ParseTKDecodeResult(String TKMsg)
        {
            TKTransactionData trx = new TKTransactionData();

            using (XmlReader reader = XmlReader.Create(new StringReader(TKMsg)))
            {
                string type = "";
                string date_time = "";
                string result = "";
                string usecs = "";
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "task")
                    {
                        type = reader.GetAttribute("type");
                        date_time = reader.GetAttribute("date_time");
                        result = reader.GetAttribute("result");
                        usecs = reader.GetAttribute("usecs");

                        if (type == "Validate")
                            trx.Task = TicketingKernel.Status.VALIDATE;
                        else trx.Task = TicketingKernel.Status.READ;

                        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(long.Parse(date_time));
                        trx.Date = date;

                        int res;
                        if (Int32.TryParse(result, out res))
                        {
                            if (res == 0)
                                trx.Result = TicketingKernel.Result.OK;
                            else trx.Result = TicketingKernel.Result.GENERAL_ERROR;
                        }

                        int t = 0;
                        Int32.TryParse(usecs, out t);
                        trx.TransactionDuration = TimeSpan.FromMilliseconds(t/1000);
                        
                        reader.ReadToFollowing("after");

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "card") 
                            {
                                String sn = reader.GetAttribute("sn");
                                int snmber;
                                if (Int32.TryParse(sn, out snmber))
                                    trx.CardSN = snmber;
                                else trx.CardSN = 0;

                                ParseActions(reader, trx);
                                break;
                            }
                        }
                    }
                }

                return trx;
            }
        }

        private void ParseActions(XmlReader reader, TKTransactionData data)
        {
            do
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "actions")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "action")
                        {
                            string attrib = reader.GetAttribute("type");
                            if (attrib == "Validate")
                            {
                                ParseProduct(reader, data);
                            }
                            break;
                        }
                    }
                }
            }
            while (reader.Read());
        }

        private void ParseProduct(XmlReader reader, TKTransactionData data)
        {
            do
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "product")
                {
                    int card_value;
                    String attrib = reader.GetAttribute("card_value");
                    if (Int32.TryParse(attrib, out card_value))
                        data.ProdId = card_value;
                    else data.ProdId = 0;
                    break;
                }
            }
            while (reader.Read());
        }
    }

    public class ProductAttribs
    {
        public DateTime date_initial { get; private set; }
        public DateTime date_final { get; private set; }
        public Double stored_value { get; private set; }

        public ProductAttribs()
        {   
        }

        public void SetDateInitialFromSec(int dateInitial)
        {
            this.date_initial = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(dateInitial);
        }

        public void SetDateFinalFromSec(int dateFinal)
        {
            this.date_final = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(dateFinal);
        }

        public void SetAmountFromCents(int amount)
        {
            this.stored_value = amount * 0.01; // 525 = 5.25
        }
    }

    public class VCardWriteOperation
    {
        public uint Address { get; set; }
        public uint Len { get; set; }
        public String Data { get; set; }

        public VCardWriteOperation() { }
    }

    public class TKResult
    {
        public uint status { get; set; }
        public uint result { get; set; }
        public string msg { get; set; }
        public IList<VCardWriteOperation> card_messages { get; set; }

        public TKResult()
        {
            card_messages = new List<VCardWriteOperation>();
        }
    }

    public class TKTransactionData
    {
        public TicketingKernel.Status Task { get; set; }
        public TicketingKernel.Result Result { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TransactionDuration { get; set; }
        public int CardSN { get; set; }
        public int ProdId { get; set; }
    }

    public class TicketingKernel
    {

        public enum Status : uint
        {
            DETECTION = 0,
            READ = 1,
            LOAD = 2,
            VALIDATE = 3,
            UNDO = 4,
            DIAGNOSTICS = 5,
            UNLOAD = 6,
            SAVE = 7,
            ISSUE = 9,
            UPDATEPROFILE = 10,
            EXTERNAL_VALID = 1000,
            TRACE = 0x544B0000,
            CANCEL = 0x544B0001,
            COUPLERINFO = 0x544B0002,
            COUPLERERROR = 0x544B0003,
            COUPLERSAMCHECK = 0x544B0004,
            COUPLERSAMADD = 0x544B0005,
            ANTENNAOFF = 0x544C0000,
            SEARCHCARD = 0x544C0001,
            CALYPSO_TXRXTPDU = 0x544C0002,
            CTS512B_READ = 0x544C0003,
            CTS512B_UPDATE = 0x544C0004
        }

        public enum Result : uint
        {
            OK = 0x00000000,
            ANTI_PASSBACK = 0x000001E1,
            BAD_CONFIG = 0x000002E1,
            BLACKLISTED_CARD = 0x000003E1,
            CARD_BLOCKED = 0x000004E1,
            CARD_EXPIRED = 0x000005E1,
            CARD_READ = 0x000006E1,
            CARD_WRITE = 0x000007E1,
            EXPIRED_JOURNEY = 0x000008E1,
            GENERAL_ERROR = 0x000009E1,
            INVALID_DATE = 0x00000AE1,
            INVALID_JOURNEY = 0x00000BE1,
            INVALID_OPERATOR = 0x00000CE1,
            INVALID_PARKING = 0x00000DE1,
            INVALID_PRODUCT = 0x00000EE1,
            INVALID_SERVICE = 0x00000FE1,
            INVALID_STOP = 0x000010E1,
            INVALID_TIME = 0x000011E1,
            NO_MORE_JOURNEYS = 0x000012E1,
            NO_MORE_TOKENS = 0x000013E1,
            NOT_AUTHORIZED = 0x000014E1,
            NOT_VALIDATED = 0x000015E1,
            NOT_YET_VALID = 0x000016E1,
            OUT_OF_DATE = 0x000017E1,
            READER_ERROR = 0x000018E1,
            SAM_ERROR = 0x000019E1,
            SAM_NOT_DETECTED = 0x00001AE1,
            CARD_EMPTY = 0x00001BE1,
            CARD_REMOVED = 0x00001CE1,
            CARD_DETECTED = 0x00001DE1,
            WRONG_CARD = 0x00001EE1,
            DISCARDED = 0x00001EE2,
            CONFLICT = 0x00001EE3,
        }
    }
}