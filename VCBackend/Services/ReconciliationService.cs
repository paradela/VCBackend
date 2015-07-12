using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.ExternalServices.Ticketing;

namespace VCBackend.Services
{
    public class ReconciliationService : IService
    {
        public ICollection<String> Transactions { private get; set; }

        public ReconciliationService(UnitOfWork uw, Device dev) : base(uw, dev) { }

        protected override bool ExecuteService()
        {
            Card4BTicketingKernelProxy tk = new Card4BTicketingKernelProxy();

            foreach (String s in Transactions)
            {
                TKTransactionData data = tk.GetTransactionData(s);
                if (data != null && data.ProdId == 2065209)
                {
                    VCardToken tok = UnitOfWork.VCardTokenRepository.Get(filter: token => (
                        token.DateInitial <= data.Date &&
                        token.DateFinal >= data.Date &&
                        !token.Used)).FirstOrDefault();
                }
            }
            return true;
        }
    }
}