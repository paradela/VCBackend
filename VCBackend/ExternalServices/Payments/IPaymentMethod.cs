using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.ExternalServices.Payments
{
    public interface IPaymentMethod
    {
        PaymentRequest PaymentBegin(PaymentRequest request);
        PaymentRequest PaymentEnd(PaymentRequest request);
        PaymentRequest PaymentCancel(PaymentRequest request);
    }
}