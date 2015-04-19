using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Payments
{
    public interface IPaymentMethod
    {
        PaymentRequest PaymentBegin(params String[] args);
        bool PaymentEnd(PaymentRequest request);
        bool PaymentCancel(PaymentRequest request);
    }
}