using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Exceptions;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public class CancelPaymentService : IService
    {
        private String method, paymentid;

        public String Method
        {
            set
            {
                method = value;
            }
        }

        public String PaymentId
        {
            set
            {
                paymentid = value;
            }
        }

        public CancelPaymentService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        public override bool ExecuteService()
        {
            if (method == null || paymentid == null)
                return false;

            PaymentRequest payment = UnitOfWork.PaymentRepository.Get(filter: q => (q.PaymentMethod == method && q.PaymentId == paymentid)).FirstOrDefault();

            if (payment != null)
            {
                if (payment.State != PaymentRequest.STATE_APPROVED)
                {
                    UnitOfWork.PaymentRepository.Delete(payment);
                }
                else throw new DeletePaymentError(String.Format("Cannot delete aproved payment!"));
            }
            else throw new PaymentNotFound(String.Format("Payment ID: {0} unknown.", paymentid));
            return true;
        }
    }
}
