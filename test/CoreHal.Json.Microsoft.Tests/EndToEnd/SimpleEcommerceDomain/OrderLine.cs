using System;

namespace CoreHal.Json.Tests.EndToEnd.SimpleEcommerceDomain
{
    public class OrderLine
    {
        public Guid OrderLineId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal LineValue { get; set; }
    }
}