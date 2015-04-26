using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class PaymentDto : IDto<PaymentRequest>
    {
        public String Amount { get; set; }
        public String Currency { get; set; }
        public String PaymentURL { get; set; }

        public void Serialize(PaymentRequest entity)
        {
            Amount = entity.Price;
            Currency = entity.Currency;
            PaymentURL = entity.RedirectURL;
        }
    }
}