using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Payments
{
    public class PayPal : IPaymentMethod
    {
        PaymentRequest IPaymentMethod.PaymentBegin(params string[] args)
        {
            throw new NotImplementedException();
        }

        bool IPaymentMethod.PaymentEnd(PaymentRequest request)
        {
            throw new NotImplementedException();
        }

        bool IPaymentMethod.PaymentCancel(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}