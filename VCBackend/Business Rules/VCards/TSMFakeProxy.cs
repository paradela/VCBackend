using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models;
using VCBackend.Repositories;

namespace VCBackend.Business_Rules.VCards
{
    public class TSMFakeProxy
    {
        public static int ERROR = 0;
        private static readonly byte[] emptyCard = new byte[]
        {
            0x60, 0x02, 0x01, 0xE4, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private VCardManager CardManager;
        private UnitOfWork UnitOfWork;

        public TSMFakeProxy()
        {
            UnitOfWork = new UnitOfWork();
            CardManager = new VCardManager(UnitOfWork);
        }

        private byte[] INT2LE(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)data;
            b[1] = (byte)(((uint)data >> 8) & 0xFF);
            b[2] = (byte)(((uint)data >> 16) & 0xFF);
            b[3] = (byte)(((uint)data >> 24) & 0xFF);
            return b;
        }

        public int InstallCard(int AccountId)
        {
            VCard card = CardManager.CreateCard(AccountId, emptyCard);
            if (card != null)
                return card.Id;
            else return ERROR;
        }

        public bool InitCard(int CardId)
        {
            if (CardId != 0)
                return CardManager.InitCard(CardId, INT2LE(CardId));
            else return false;
        }
    }
}