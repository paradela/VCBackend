using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Repositories;

namespace VCBackend.Business_Rules.VCard
{
    public class VCardManager
    {
        private static readonly byte[] emptyCard = new byte[]
        {
            0x60, 0x02, 0x01, 0xE4, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private static VCardManager CardManager = null;

        private IRepository<VCBackend.Models.VCard> rep { get; set; }

        private VCardManager()
        {
            rep = VCardRepository.getRepositorySingleton();
        }

        public static VCardManager getVCardManagerSingleton()
        {
            if (CardManager == null)
            {
                CardManager = new VCardManager();
            }
            return CardManager;
        }

        //Create Card
        public VCBackend.Models.VCard CreateCard()
        {
            VCBackend.Models.VCard card = new Models.VCard(emptyCard);
            rep.Add(card);
            return card;
        }
        
        //Init Card (Add Serial number)
        public bool InitCard(int CId, byte[] SerialNumber)
        {
            Models.VCard card = rep.FindById(CId);
            if (card != null || SerialNumber.Length == 4)
            {
                return card.Write(6, SerialNumber, (uint)SerialNumber.Length);
            }
            else return false;
        }
        //Get Card
        //Delete Card
    }
}