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

        private void LoadProductEx(String ProdId, VCard Card = null, VCardToken Token = null)
        {
            var response = Client.GetAsync(ServerUri).Result;
            string obj = response.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(obj);
            string tkUri = json["URL"].ToString();

            response = Client.PostAsync(tkUri, new StringContent("")).Result;
            obj = response.Content.ReadAsStringAsync().Result;
            json = JObject.Parse(obj);
        }
    }
}