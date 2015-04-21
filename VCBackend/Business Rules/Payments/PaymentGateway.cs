using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Payments
{
    public class PaymentGateway
    {
        private IDictionary<String, object> methods;

        public PaymentGateway()
        {
            methods = new Dictionary<String, object>();
            methods["paypal"] = new PayPal();
        }

        public object GetPaymentMethodByName(String name)
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

    public class PaymentMethodUnknown : Exception
    {
        public PaymentMethodUnknown() { }
        public PaymentMethodUnknown(String Message) : base(Message) { }
    }
}