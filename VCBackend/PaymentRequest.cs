//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VCBackend
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaymentRequest
    {
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public string State { get; set; }
        public string ProductId { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string RedirectURL { get; set; }
        public string PaymentData { get; set; }
        public string PayerId { get; set; }
        public int AccountId { get; set; }
    }
}