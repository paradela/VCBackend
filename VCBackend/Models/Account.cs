using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Exceptions;
using VCBackend.Utility.Security;
using VCBackend.ExternalServices.TSM;
using VCBackend.Repositories;

namespace VCBackend
{
    public partial class Account
    {
        private static double MIN_AMOUNT = 5.0;

        public Account(Double Balance)
        {
            this.Balance = Balance;
        }

        public bool InitializeAccount()
        {
            TSMFakeProxy proxy = new TSMFakeProxy();

            int card = proxy.InstallCard(Id);

            if (card != TSMFakeProxy.ERROR)
            {
                return proxy.InitCard(card);
            }
            else return false;
        }

        public VCard CreateCard(byte[] EmptyCard)
        {
            VCard card = new VCard(EmptyCard);
            this.VCard = card;
            card.AccountId = this.Id;
            return card;
        }

        public Double AddFunds(String NewAmount)
        {
            Double amount = 0.0;

            String str = NewAmount.Replace(",", "");
            str = str.Replace(".", ",");

            amount = Double.Parse(str);

            return AddFunds(amount);
        }

        public Double AddFunds(Double amount)
        {
            if (amount < MIN_AMOUNT)
                throw new InvalidAmountException(
                    String.Format("The given amount is below the minimum value of {0}", MIN_AMOUNT));
            this.Balance += amount;
            return this.Balance;
        }

        public Double Withdraw(Double Price)
        {
            if (Price >= 0 && Price <= Balance)
            {
                this.Balance -= Price;
                return this.Balance;
            }
            else throw new InvalidAmountException("This account has insufficient funds for this operation.");
        }

        public String GetAuthToLoadCard()
        {
            if (VCard.Id != 0)
                return AuthToken.GetCardAccessJwt(VCard);
            else throw new CardNotFound(String.Format("The VCard isn't created."));
        }

        public String GetAuthToLoadCard(int cardId)
        {
            if (cardId != 0)
            {
                var VCard = (from c in VCardToken
                             where c.Id == cardId
                             select c).FirstOrDefault();
                if (VCard == null) throw new CardNotFound(String.Format("Invalid VCard Token identifier.")); 

                return AuthToken.GetCardTokenAccessJwt(VCard);
            }
            else throw new CardNotFound(String.Format("The VCard Token isn't created."));
        }
    }
}