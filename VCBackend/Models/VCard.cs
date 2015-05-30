using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Utility.Time;

namespace VCBackend
{
    public partial class VCard
    {
        public VCard() { }
        public VCard(byte[] Data)
        {
            this.Data = System.Convert.ToBase64String(Data);
        }

        public byte[] Read(uint Offset, uint Length)
        {
            byte[] card = System.Convert.FromBase64String(this.Data);

            if ((Offset + Length) <= card.Length && Length > 0)
            {
                byte[] dataRead = new byte[Length];
                System.Array.Copy(card, Offset, dataRead, 0, Length);
            }
            return new byte[0];
        }

        public bool Write(uint OffSet, byte[] Data, uint Length)
        {
            byte[] card = System.Convert.FromBase64String(this.Data);

            if ((OffSet + Length) <= card.Length)
            {
                System.Array.Copy(Data, 0, card, OffSet, Length);
                this.Data = System.Convert.ToBase64String(card);
                return true;
            }
            else return false;
        }

        public byte[] GetBytes()
        {
            if (this.Data == null) return new byte[0];
            byte[] card = System.Convert.FromBase64String(this.Data);
            return card;
        }

        public bool InitCard(byte[] SerialNumber)
        {
            if (SerialNumber.Length == 4)
                return Write(6, SerialNumber, (uint)SerialNumber.Length);
            else return false;
        }

        public VCardToken CreateVCardToken(Account account)
        {
            byte[] cardData = new byte[64];
            byte[] cardHeader = Read(0, 10); //Read the card header till the S/N.
            System.Array.Copy(cardHeader, cardData, 10);
            VCardToken token = new VCardToken(cardData);
            token.AccountId = account.Id;
            account.VCardToken.Add(token);
            return token;
        }
    }
}