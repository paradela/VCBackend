using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCBackend.Models.Dto;
using VCBackend.Repositories;

namespace VCBackend.Services
{
    public class RunApduService : IService
    {
        public ApduResponseDto ResponseDto { get; private set; }
        public String B64Apdu { private get; set; }

        public RunApduService(UnitOfWork uw, Device dev)
            : base(uw, dev) { }

        protected override bool ExecuteService()
        {
            VCard card = AuthDevice.Owner.Account.VCard;
            byte[] apdu = System.Convert.FromBase64String(B64Apdu);
            byte[] res = card.IsoATxRxAPDU(apdu);
            UnitOfWork.Save();
            ResponseDto = new ApduResponseDto();
            ResponseDto.B64Response = System.Convert.ToBase64String(res);
            return true;
        }
    }
}