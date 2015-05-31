using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Utility.Time;

namespace VCBackend
{
    public partial class VCard
    {
        private static String AID = "00A404000A43344243545335313242";
        public VCard() { }
        public VCard(byte[] Data)
        {
            this.Data = System.Convert.ToBase64String(Data);
        }

        public byte[] Read(uint Address, uint Length)
        {
            byte[] card = System.Convert.FromBase64String(this.Data);

            if ((Address + Length) <= card.Length && Length > 0)
            {
                byte[] dataRead = new byte[Length];
                System.Array.Copy(card, Address, dataRead, 0, Length);
                return dataRead;
            }
            return new byte[0];
        }

        public bool Write(uint Address, byte[] Data, uint Length)
        {
            byte[] card = System.Convert.FromBase64String(this.Data);

            if ((Address + Length) <= card.Length)
            {
                System.Array.Copy(Data, 0, card, Address, Length);
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

        public byte[] IsoATxRxAPDU(byte[] APDU)
        {
            byte[] result = new byte[] { 0x6B, 0x00 };

            if (APDU != null && APDU.Length > 0)
            {
                byte[] vAID = StringToByteArray(AID);
                if (System.Array.Equals(APDU, vAID))
                {
                    result = new byte[] { 0x43, 0x34, 0x42, 0x43, 0x54, 0x53, 0x35, 0x31, 0x32, 0x42, 0x90, 0x00 };
                }
                else if (IsReadAPDU(APDU))
                {
                    uint address = APDU[3];
                    uint length = APDU[4];
                    byte[] read = Read(address, length);
                    if (read.Length == length)
                    {
                        result = new byte[length + 2];
                        System.Array.Copy(read, result, length);
                        result[result.Length - 1] = 0x00;
                        result[result.Length - 2] = 0x90;
                    }
                }
                else if (IsWriteAPDU(APDU))
                {
                    uint address = APDU[3];
                    uint length = APDU[4];
                    byte[] data = new byte[length];
                    System.Array.Copy(APDU, 5, data, 0, length);
                    if (Write(address, data, length))
                        result = new byte[] { 0x90, 0x00 };
                }
                    
            }
            return result;

        }

        private bool IsReadAPDU(byte[] APDU)
        {
            if (APDU != null && 
                APDU.Length == 5 &&
                APDU[0] == 0x00 &&
                APDU[1] == 0xB0 &&
                APDU[2] == 0x00
                )
                return true;
            else return false;
        }

        private bool IsWriteAPDU(byte[] APDU)
        {
            if (APDU != null &&
                APDU.Length > 5 &&
                APDU[0] == 0x00 &&
                APDU[1] == 0xD6 &&
                APDU[2] == 0x00
                )
                return true;
            else return false;
        }

        //From: http://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}