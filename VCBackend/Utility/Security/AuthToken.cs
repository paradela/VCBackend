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

        private static int VALIDITY = 1800; // 1 hour

        public static String GenerateToken(User User, Device Device)
        {
            var now = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

            var payload = new Dictionary<string, object>()
            {
                { "user_id", User.Id },
                { "device_id", Device.Id },
                { "timestamp", now }
            };

            
            if (Device.Type == DeviceType.DEFAULT_DEVICE)
            {
                payload["exp"] = now + VALIDITY;
            }

            string token = JWT.JsonWebToken.Encode(payload, AuthTokenSecret, JWT.JwtHashAlgorithm.HS512);

            return token;
        }

        public static bool ValidateToken(String Token)
        {
            try
            {
                JWT.JsonWebToken.Decode(Token, AuthTokenSecret, true);
                //didn't threw an exception, is valid!!
                return true;
            }
            catch (JWT.SignatureVerificationException)
            {
                return false;
            }

        }
    }
}