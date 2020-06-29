using System;

namespace CoreHal.Json.Tests.EndToEnd.SimpleEcommerceDomain
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public string PrimaryContact { get; set; }
        public Sector Sector { get; set; }
    }
}