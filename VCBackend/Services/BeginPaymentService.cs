using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCBackend.Repositories;
using VCBackend.Exceptions;
using VCBackend.Models.Dto;
using VCBackend.ExternalServices.Payments;

namespace VCBackend.Services
{
    public class BeginPaymentService : IService
    {
        private String amount;
        private String method;
        private PaymentDto dto;

        public String Amount
        {
            set
            {
                amount = value;
            }
        }

        public String Method
        {
            set
            {
                method = value;
            }
        }

        public PaymentDto PaymentDto
        {
            get
            {
                return dto;
            }
        }

        public BeginPaymentService(UnitOfWork UnitOfWork, Device AuthDevice)
            : base(UnitOfWork, AuthDevice) { }

        protected override bool ExecuteService()
        {
            if (amount == null || method == null)
                return false;

            Account Account = AuthDevice.Owner.Account;

            PaymentGateway gateway = new PaymentGateway();
            IPaymentMethod payMethod = gateway.GetPaymentMethodByName(method);

            PaymentRequest request = new PaymentRequest();
            request.PaymentMethod = method;
            request.Currency = "EUR"; //Maybe this should be a server definition.
            request.Price = amount;

            PaymentRequest newRequest = payMethod.PaymentBegin(request);
            
            Account.PaymentRequest.Add(newRequest);

            UnitOfWork.AccountRepository.Update(Account);
            UnitOfWork.Save();

            dto = new PaymentDto();
            dto.Serialize(newRequest);

            return true;
        }
    }
}
