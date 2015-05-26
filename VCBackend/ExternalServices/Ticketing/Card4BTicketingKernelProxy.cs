using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;

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

        public void LoadProduct(String ProdId, VCard Card)
        {
            LoadProductEx(ProdId, Card: Card);
        }

        public void LoadProduct(String ProdId, VCardToken Token)
        {
            LoadProductEx(ProdId, Token: Token);
        }

        private async void LoadProductEx(String ProdId, VCard Card = null, VCardToken Token = null)
        {
            //Call WS to get a server to load the card
            var callTask = Client.GetAsync(ServerUri);

            var card = new JObject();

            if (Card == null)
            {
                card["type"] = "CTS512B";
                card["data"] = "carddadatasdasda";//Card.Data;
            }
            else if (Token != null)
            {
                card["type"] = "CTS512B";
                card["data"] = Token.Data;
            }
            else return;

            await callTask; //wait for call to be finnished

            string strRsp = callTask.Result.Content.ReadAsStringAsync().Result;
            var jsonRsp = JObject.Parse(strRsp);
            string tkUri = jsonRsp["URL"].ToString();

            var jsonReq = new JObject();
            jsonReq["tkmsg"] = "<tkmsg><product>" + ProdId + "</product></tkmsg>";
            jsonReq["card"] = card;
            var response = Client.PostAsJsonAsync(tkUri, jsonReq).Result;
            strRsp = response.Content.ReadAsStringAsync().Result;
            jsonRsp = JObject.Parse(strRsp);
        }
    }
}