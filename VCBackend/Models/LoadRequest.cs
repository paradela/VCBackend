using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class LoadRequest
    {
        public static string STATE_CREATED = "created";
        public static string STATE_APPROVED = "approved";
        public static string STATE_FAILED = "failed";
        public static string STATE_SUCCESS = "success";
        public static int ERROR_UNKNOWN = -1;

        public LoadRequest()
        {
            this.SaleDate = DateTime.Now;
            this.State = STATE_CREATED;
            this.LoadResult = ERROR_UNKNOWN;
        }

        public bool ApproveLoad()
        {
            if (State == STATE_CREATED)
            {
                this.State = STATE_APPROVED;
                return true;
            }
            return false;
        }

        public void FailedLoad(int Result = -1)
        {
            if (State == STATE_APPROVED || State == STATE_CREATED)
            {
                this.State = STATE_FAILED;
                if (Result > ERROR_UNKNOWN) this.LoadResult = Result;
            }
        }

        public void SuccessfullLoad()
        {
            if (State == STATE_APPROVED)
            {
                this.State = STATE_SUCCESS;
                this.LoadResult = 0;
            }
        }
    }
}