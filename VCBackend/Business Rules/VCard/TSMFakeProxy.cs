using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.VCard
{
    public class TSMFakeProxy
    {
        private static TSMFakeProxy Tsm = null;

        private VCardManager CardManager { get; set; }

        private TSMFakeProxy()
        {
            //get cardManager singleton
        }

        public static TSMFakeProxy getTSMFakeProxySingleton()
        {
            if (Tsm == null)
            {
                Tsm = new TSMFakeProxy();
            }
            return Tsm;
        }


        //Install
        //Init
    }
}