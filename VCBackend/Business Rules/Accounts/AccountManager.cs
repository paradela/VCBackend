using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Business_Rules.VCards;
using VCBackend.Utility.Security;
using VCBackend.Business_Rules.Payments;
using VCBackend.Exceptions;

namespace VCBackend.Business_Rules.Accounts
{
    public class AccountManager : IManager
    {
        public AccountManager(UnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {
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

        

        public PaymentRequest PaymentBegin(Account Account, String Method, String Currency, String Amount)
        {
            PaymentGateway gateway = new PaymentGateway();
            IPaymentMethod payMethod = gateway.GetPaymentMethodByName(Method);

            PaymentRequest request = new PaymentRequest();
            request.PaymentMethod = Method;
            request.Currency = Currency;
            request.Price = Amount;

            PaymentRequest newRequest = payMethod.PaymentBegin(request);

            Account.PaymentRequest.Add(newRequest);

            UnitOfWork.AccountRepository.Update(Account);
            UnitOfWork.Save();

            return newRequest;
        }

        public PaymentRequest PaymentEnd(Account Account, String Method, String PayerId, String PaymentId)
        {
            PaymentGateway gateway = new PaymentGateway();
            IPaymentMethod payMethod = gateway.GetPaymentMethodByName(Method);
            PaymentRequest newRequest = null;

            var request = (from payment in Account.PaymentRequest
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

                    Account.AddFunds(request.Price);
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
            PaymentRequest payment = UnitOfWork.PaymentRepository.Get(filter: q => (q.PaymentMethod == Method && q.PaymentId == PaymentId)).FirstOrDefault();

            if (payment != null)
            {
                if (payment.State != PaymentRequest.STATE_APPROVED)
                {
                    UnitOfWork.PaymentRepository.Delete(payment);
                }
                else throw new DeletePaymentError(String.Format("Cannot delete aproved payment!"));
            }
            else throw new PaymentNotFound(String.Format("Payment ID: {0} unknown.", PaymentId));
        }

        public String GetNewToken(Account Account, String ProductId, DateTime DateInitial)
        {
            VCardManager CardManager = new VCardManager(UnitOfWork);
            ProductManager ProdManager = new ProductManager(UnitOfWork);

            User u = UnitOfWork.UserRepository.Get(filter: q => (q.Account.Id == Account.Id)).FirstOrDefault();

            if (u == null) 
                throw new AccountNotFound("Invalid Account identifier.");
            if (ProductId == null || ProductId == String.Empty)
                throw new VCException("Invalid Product Identifier.");
            if (DateInitial == null || DateInitial < DateTime.Now)
                throw new VCException("Invalid initial date.");

            VCardToken token;

            using (var transaction = UnitOfWork.TransactionBegin())
            {
                token = CardManager.CreateTokenFromCard(Account.VCard.Id, Account.Id);

                LoadRequest req = new LoadRequest();
                req.CardAuth = GetAuthToLoadCard(token);
                req.DateInitial = DateInitial;
                req.ProdId = ProductId;
                req.Quantity = 1;
                //ProdManager

                
            }

            VCardSecurity secure = new VCardSecurity(u);

            token = UnitOfWork.VCardTokenRepository.GetByID(token.Id);

            String secureToken = secure.RijndaelEncrypt(token);


            return secureToken;
        }
    }
}