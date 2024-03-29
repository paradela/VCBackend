﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Exceptions;

namespace VCBackend.ExternalServices.Payments
{
    public class PaymentGateway
    {
        private IDictionary<String, IPaymentMethod> methods;

        public PaymentGateway()
        {
            methods = new Dictionary<String, IPaymentMethod>();
            methods["paypal"] = new PayPalMethod();
        }

        public IPaymentMethod GetPaymentMethodByName(String name)
        {
            var method = methods[name];
            if (method == null)
            {
                throw new PaymentMethodUnknown(String.Format("Payment method {0} unknown.", name));
            }
            return method;
        }

        public IEnumerable<String> PaymentMethods 
        {
            get
            {
                return methods.Keys;
            }
        }
    }

    public class PaymentMethodUnknown : VCException
    {
        public PaymentMethodUnknown(String Message) : base(Message) 
        {
            Error = "PaymentMethodUnknown";
        }
    }
}