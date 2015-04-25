using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;
using VCBackend.Models;
using VCBackend.Business_Rules.Accounts;
using VCBackend.Utility.Time;
using VCBackend.Business_Rules.Exceptions;

namespace VCBackend.Business_Rules.VCards
{
    public class VCardManager : IManager
    {
        public VCardManager(UnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {
        }

        //Create Card
        public VCard CreateCard(int AccountId, byte[] EmptyCard)
        {
            AccountManager manager = new AccountManager(UnitOfWork);
            VCard card = new VCard(EmptyCard);

            UnitOfWork.VCardRepository.Add(card);
            if (!manager.SetCardToAccount(AccountId, card))
            {
                UnitOfWork.VCardRepository.Delete(card);
                return null;
            }
            UnitOfWork.Save();
            return card;
        }
        
        //Init Card (Add Serial number)
        public bool InitCard(int CardId, byte[] SerialNumber)
        {
            VCard card = UnitOfWork.VCardRepository.GetByID(CardId);
            if (card != null || SerialNumber.Length == 4)
            {
                if (card.Write(6, SerialNumber, (uint)SerialNumber.Length))
                {
                    UnitOfWork.VCardRepository.Update(card);
                    UnitOfWork.Save();
                    return true;
                }
            }
            
            return false;
        }

        //Get Card
        public VCard GetCardById(int CardId)
        {
            VCard card = UnitOfWork.VCardRepository.GetByID(CardId);
            if (card == null)
            {
                throw new CardNotFound("Card ID unknown.");
            }
            return card;
        }

        //Delete Card
        public void DeleteCardById(int CardId)
        {
            VCard card = UnitOfWork.VCardRepository.GetByID(CardId);
            if (card == null)
            {
                throw new CardNotFound("Card ID unknown.");
            }
            UnitOfWork.VCardRepository.Delete(card);
            UnitOfWork.Save();
        }

        public VCardToken CreateTokenFromCard(int CardId, int AccountId)
        {
            VCard card = UnitOfWork.VCardRepository.GetByID(CardId);
            Account account = UnitOfWork.AccountRepository.GetByID(AccountId);
            if (card == null) throw new CardNotFound("Card ID unknown.");
            if (account == null) throw new AccountNotFound("Account ID unknown");
            byte[] cardData = new byte[64];
            byte[] cardHeader = card.Read(0, 10); //Read the card header till the S/N.
            System.Array.Copy(cardHeader, cardData, 10);
            VCardToken token = new VCardToken(cardData);
            token.AccountId = card.AccountId;
            token.Validity = Time.ValidityInHours(24);
            account.VCardToken.Add(token);
            UnitOfWork.VCardTokenRepository.Add(token);
            UnitOfWork.AccountRepository.Update(account);
            UnitOfWork.Save();
            return token;
        }
    }
}