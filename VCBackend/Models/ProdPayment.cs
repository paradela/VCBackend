using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend
{
    public partial class PaymentRequest
    {
        public static string STATE_CREATED = "created";
        public static string STATE_APPROVED = "approved";
        public static string STATE_FAILED = "failed";
        public static string STATE_CANCELED = "canceled";
        public static string STATE_EXPIRED = "expired";
        public static string STATE_PENDING = "pending";
    }
}