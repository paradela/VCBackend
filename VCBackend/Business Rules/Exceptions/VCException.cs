using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Business_Rules.Exceptions
{
    public class VCException : Exception
    {
        public String Error { get; set; }
        public String Description { get; set; }

        public VCException(String Description)
        {
            this.Description = Description;
        }
    }
    /********************************************/
    //           Exceptions                     //
    /*******************************************/
    public class CardNotFound : VCException
    {
        public CardNotFound(String Message)
            : base(Message)
        {
            Error = "CardNotFound";
        }
    }

    public class DeviceNotFound : VCException
    {
        public DeviceNotFound(String Message)
            : base(Message)
        {
            Error = "DeviceNotFound";
        }
    }

    public class AccountNotFound : VCException
    {
        public AccountNotFound(String Message)
            : base(Message)
        {
            Error = "AccountNotFound";
        }
    }

    public class EmailAlreadyRegistered : VCException
    {
        public EmailAlreadyRegistered(String Message)
            : base(Message)
        {
            Error = "EmailAlreadyRegistered";
        }
    }

    public class DeviceAlreadyRegistered : VCException
    {
        public DeviceAlreadyRegistered(String Message)
            : base(Message)
        {
            Error = "DeviceAlreadyRegistered";
        }
    }

    public class InvalidCredentials : VCException
    {
        public InvalidCredentials(String Message)
            : base(Message)
        {
            Error = "InvalidCredentials";
        }
    }

    public class InvalidDataFormat : VCException
    {
        public InvalidDataFormat(String Message)
            : base(Message)
        {
            Error = "InvalidDataFormat";
        }
    }

    public class PayPalPaymentFailed : VCException
    {

        public PayPalPaymentFailed(String message)
            : base(message)
        {
            Error = "PayPalPaymentFailed";
        }
    }
}