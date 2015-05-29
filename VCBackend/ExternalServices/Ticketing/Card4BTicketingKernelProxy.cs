using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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

        public bool LoadCard(VCard Card, uint Ammount, uint LoadId)
        {
            if (Ammount >= 500)
            {
                string tkmsg = 
                    "<tkmsg><load>" +
                    "<product card_value=\"2065208\"><attribs>" +
                    "<attrib type=\"stored_value\">" + Ammount + "</attrib>" +
                    "<attrib type=\"sale_device\">42</attrib>" +
                    "<attrib type=\"sale_date\">" + DateTime.Today.ToString("yyyy-MM-dd") + "</attrib>" +
                    "<attrib type=\"sale_number_daily\">" + LoadId + "</attrib>" +
                    "</attribs></product></load></tkmsg>";
                return LoadProductEx(tkmsg, Card: Card).Result;
            }
            else return false;
        }

        public bool LoadToken(VCardToken Token, uint LoadId)
        {
            string tkmsg =
                "<tkmsg><load>" +
                "<product card_value=\"2065209\"><attribs>" +
                "<attrib type=\"stored_value\">100</attrib>" +
                "<attrib type=\"sale_device\">42</attrib>" +
                "<attrib type=\"sale_date\">" + DateTime.Today.ToString("yyyy-MM-dd") + "</attrib>" +
                "<attrib type=\"sale_number_daily\">" + LoadId + "</attrib>" +
                "<attrib type=\"date_initial\">" + DateTime.Today.ToString("yyyy-MM-dd HH:mm") + "</attrib>" +
                "</attribs></product></load></tkmsg>";
            return LoadProductEx(tkmsg, Token: Token).Result;
        }

        private async Task<bool> LoadProductEx(String TKMsg, VCard Card = null, VCardToken Token = null)
        {
            //Call WS to get a server to load the card
            var callTask = Client.GetAsync(ServerUri);

            var card = new JObject();

            if (Card != null)
            {
                card["type"] = "CTS512B";
                card["data"] = Card.Data;//Card.Data;
            }
            else if (Token != null)
            {
                card["type"] = "CTS512B";
                card["data"] = Token.Data;
            }
            else return false;

            await callTask; //wait for call to be finnished

            string strRsp = callTask.Result.Content.ReadAsStringAsync().Result;
            var jsonRsp = JObject.Parse(strRsp);
            string tkUri = jsonRsp["URL"].ToString();

            var jsonReq = new JObject();
            jsonReq["tkmsg"] = TKMsg;
            jsonReq["card"] = card;
            var response = Client.PostAsJsonAsync(tkUri, jsonReq).Result;
            strRsp = response.Content.ReadAsStringAsync().Result;
            jsonRsp = JObject.Parse(strRsp);
            return true;
        }
    }
}