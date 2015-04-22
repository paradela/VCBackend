using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Payments
{
    public interface IPaymentMethod
    {
        ProdPayment PaymentBegin(ProdPayment request);
        ProdPayment PaymentEnd(ProdPayment request);
        ProdPayment PaymentCancel(ProdPayment request);
    }
}