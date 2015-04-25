using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Business_Rules.VCards;
using VCBackend.Utility.Security;
using VCBackend.Business_Rules.Payments;
using VCBackend.Business_Rules.Exceptions;

namespace VCBackend.Business_Rules.Accounts
{
    public class AccountManager : IManager
    {
        public AccountManager(UnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {
        }

        public Account CreateAccount()
        {
            Account account = new Account();
            account.Balance = 0.0;
            UnitOfWork.AccountRepository.Add(account);
            return account;
        }

        public bool InitializeAccount(int AccountId)
        {
            Account Account = UnitOfWork.AccountRepository.GetByID(AccountId);

            if (Account == null) return false;

            TSMFakeProxy proxy = new TSMFakeProxy();

            int card = proxy.InstallCard(Account.Id);

            if (card != TSMFakeProxy.ERROR)
            {
                return proxy.InitCard(card);
            }
            else return false;
        }

        public bool SetCardToAccount(int AccountId, VCard NewCard)
        {
            Account Account = UnitOfWork.AccountRepository.GetByID(AccountId);
            if (Account == null) return false;
            NewCard.AccountId = AccountId;
            Account.VCard = NewCard;
            UnitOfWork.AccountRepository.Update(Account);
            UnitOfWork.Save();
            return true;
        }

        public String GetAuthToLoadCard(Account Account)
        {
            if (Account.VCard.Id != 0)
                return AuthToken.GetCardAccessJwt(Account.VCard);
            else throw new CardNotFound(String.Format("The card isn't created."));
        }

        public ProdPayment PaymentBegin(Account Account, String Method, String Currency, String Amount)
        {
            PaymentGateway gateway = new PaymentGateway();
            IPaymentMethod payMethod = gateway.GetPaymentMethodByName(Method);

            ProdPayment request = new ProdPayment();
            request.PaymentMethod = Method;
            request.Currency = Currency;
            request.Price = Amount;

            ProdPayment newRequest = payMethod.PaymentBegin(request);

            Account.ProdPayments.Add(newRequest);

            UnitOfWork.AccountRepository.Update(Account);
            UnitOfWork.Save();

            return newRequest;
        }

        public ProdPayment PaymentEnd(Account Account, String Method, String PayerId, String PaymentId)
        {
            PaymentGateway gateway = new PaymentGateway();
            IPaymentMethod payMethod = gateway.GetPaymentMethodByName(Method);
            ProdPayment newRequest = null;

            var request = (from payment in Account.ProdPayments
                          where (payment.PaymentId == PaymentId && payment.AccountId == Account.Id)
                          select payment).FirstOrDefault();

            if (request == null) throw new PaymentNotFound(String.Format("No payment request registered with ID {0}.", PaymentId));

            using (var transactionCtx = UnitOfWork.TransactionBegin())
            {
                try
                {
                    request.PayerId = PayerId;
                    newRequest = payMethod.PaymentEnd(request);
                    UnitOfWork.PaymentRepository.Update(newRequest);

                    Account.AddBalance(request.Price);
                    UnitOfWork.AccountRepository.Update(Account);

                    UnitOfWork.Save();

                    transactionCtx.Commit();
                }
                catch(Exception ex) {
                    transactionCtx.Rollback();
                    throw ex;
                }

            }

            return newRequest;
        }

        public void PaymentCancel(Account Account, String Method, String PaymentId)
        {
            ProdPayment payment = UnitOfWork.PaymentRepository.Get(filter: q => (q.PaymentMethod == Method && q.PaymentId == PaymentId)).FirstOrDefault();

            if (payment != null)
            {
                if (payment.State != ProdPayment.STATE_APPROVED)
                {
                    UnitOfWork.PaymentRepository.Delete(payment);
                }
                else throw new DeletePaymentError(String.Format("Cannot delete aproved payment!"));
            }
            else throw new PaymentNotFound(String.Format("Payment ID: {0} unknown.", PaymentId));
        }

    }
}