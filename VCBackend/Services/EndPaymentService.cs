using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Models.Dto;
using VCBackend.ExternalServices.Payments;
using VCBackend.Exceptions;

namespace VCBackend.Services
{
    public class EndPaymentService : IService
    {
        private String method, payerid, paymentid;
        private BalanceDto dto;

        public String Method
        {
            set
            {
                method = value;
            }
        }

        public String PayerId
        {
            set
            {
                payerid = value;
            }
        }

        public String PaymentId
        {
            set
            {
                paymentid = value;
            }
        }

        public BalanceDto BalanceDto
        {
            get
            {
                return dto;
            }
        }

        public EndPaymentService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        protected override bool ExecuteService()
        {
            if (method == null || payerid == null || paymentid == null)
                return false;
            PaymentGateway gateway = new PaymentGateway();
            IPaymentMethod payMethod = gateway.GetPaymentMethodByName(method);
            PaymentRequest newRequest = null;
            Account Account = AuthDevice.Owner.Account;

            var request = (from payment in Account.PaymentRequest
                           where (payment.PaymentId == paymentid && payment.AccountId == Account.Id)
                           select payment).FirstOrDefault();

            if (request == null) throw new PaymentNotFound(String.Format("No payment request registered with ID {0}.", paymentid));

            request.PayerId = payerid;
            newRequest = payMethod.PaymentEnd(request);
            UnitOfWork.PaymentRepository.Update(newRequest);

            Account.AddFunds(request.Price);
            UnitOfWork.AccountRepository.Update(Account);

            UnitOfWork.Save();

            dto = new BalanceDto();
            dto.Serialize(Account);
            return true;
        }
    }
}
