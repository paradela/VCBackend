using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Business_Rules.VCards;
using VCBackend.Utility.Security;

namespace VCBackend.Business_Rules.Accounts
{
    public class AccountManager
    {
        private static AccountManager accountManager = null;

        private IRepository<Account> rep;

        private AccountManager()
        {
            rep = AccountRepository.getRepositorySingleton();
        }

        public static AccountManager getManagerSingleton()
        {
            if (accountManager == null)
                accountManager = new AccountManager();
            return accountManager;
        }

        public Account CreateAccount()
        {
            Account account = new Account();

            return account;
        }

        public bool InitializeAccount(int AccountId)
        {
            Account Account = rep.FindById(AccountId);

            if (Account == null) return false;

            TSMFakeProxy proxy = TSMFakeProxy.getTSMFakeProxySingleton();

            int card = proxy.InstallCard(Account.Id);

            if (card != TSMFakeProxy.ERROR)
            {
                return proxy.InitCard(card);
            }
            else return false;
        }

        public bool SetCardToAccount(int AccountId, VCard NewCard)
        {
            Account Account = rep.FindById(AccountId);
            if (Account == null) return false;
            Account.Card = NewCard;
            rep.Update(Account);
            return true;
        }

        public void UseOnlineValidation(Account Account)
        {
            //Load Card with Account.Balance
        }

        public void UseTokenValidation(Account Account)
        {
            //Convert balance present in the card to Account.Balance
        }

        public String GetAuthToLoadCard(Account Account)
        {
            if (Account.Type == ValidationType.ONLINE)
                return AuthToken.GetCardAccessToken(Account.Card);
            else throw new Exception(); // Create Exception
        }

        public String GetAuthToCreateToken(Account Account)
        {
            if (Account.Type == ValidationType.TOKENIZED)
                return "token Account.Card.Id";
            else throw new Exception(); // Create Exception
        }
    }
}