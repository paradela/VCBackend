﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Business_Rules.VCards;
using VCBackend.Utility.Security;

namespace VCBackend.Business_Rules.Accounts
{
    public class AccountManager : Manager
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
            if (Account.IsOnline)
                return AuthToken.GetCardAccessJwt(Account.VCard);
            else throw new Exception(); // Create Exception
        }

        public String GetAuthToCreateToken(Account Account)
        {
            if (!Account.IsOnline)
                return "token Account.Card.Id";
            else throw new Exception(); // Create Exception
        }
    }
}