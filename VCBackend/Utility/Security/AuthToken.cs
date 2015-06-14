using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;

namespace VCBackend.Utility.Security
{
    public class AuthToken
    {
        //should be loaded from a file!
        private static string AuthTokenSecret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

        private static int API_AUTH_TOKEN_VALIDITY_S = 1800; // 1 hour
        private static int API_REFRESH_TOKEN_VALIDITY_M = 6; // 6 months
        private static int CARD_KEY_VALIDITY = 60; // 1 min

        private static String API_ACCESS_TOKEN_TYPE_AUTH = "AUTH";
        private static String API_ACCESS_TOKEN_TYPE_REFRESH = "REFRESH";

        public static String GetAPIAuthJwt(User User, Device Device)
        {
            var now = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            var payload = new Dictionary<string, object>()
            {
                {      "type", API_ACCESS_TOKEN_TYPE_AUTH      },
                {   "user_id", User.Id                         },
                { "device_id", Device.Id                       },
                { "timestamp", now                             },
                {       "exp", now + API_AUTH_TOKEN_VALIDITY_S }
            };

            string token = JWT.JsonWebToken.Encode(payload, AuthTokenSecret, JWT.JwtHashAlgorithm.HS512);

            return token;
        }

        public static String GetAPIRefreshJwt(User User, Device Device)
        {
            var utcNow = DateTime.UtcNow;
            var now = Math.Round((utcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            var validity = Math.Round((utcNow.AddMonths(API_REFRESH_TOKEN_VALIDITY_M) - 
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            var payload = new Dictionary<string, object>()
            {
                {      "type", API_ACCESS_TOKEN_TYPE_REFRESH },
                {   "user_id", User.Id                       },
                { "device_id", Device.Id                     },
                { "timestamp", now                           },
                {       "exp", now + validity                }
            };

            string token = JWT.JsonWebToken.Encode(payload, AuthTokenSecret, JWT.JwtHashAlgorithm.HS512);

            return token;
        }

        public static IDictionary<string, object> ValidateToken(String Token)
        {
            try
            {
                var payload = JWT.JsonWebToken.DecodeToObject(Token, AuthTokenSecret, true) 
                    as IDictionary<string, object>;
                //didn't threw an exception, is valid!!
                return payload;
            }
            catch (JWT.SignatureVerificationException)
            {
                return null;
            }

        }

        public static String GetCardAccessJwt(VCard Card)
        {
            var now = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            var payload = new Dictionary<string, object>()
            {
                { "type", "card"},
                { "card_id", Card.Id },
                { "exp", now + CARD_KEY_VALIDITY }
            };

            string token = JWT.JsonWebToken.Encode(payload, AuthTokenSecret, JWT.JwtHashAlgorithm.HS512);

            return token;
        }

        public static String GetCardTokenAccessJwt(VCardToken Card)
        {
            var now = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            var payload = new Dictionary<string, object>()
            {
                { "type", "token"},
                { "card_id", Card.Id },
                { "exp", now + CARD_KEY_VALIDITY }
            };

            string token = JWT.JsonWebToken.Encode(payload, AuthTokenSecret, JWT.JwtHashAlgorithm.HS512);

            return token;
        }
    }
}