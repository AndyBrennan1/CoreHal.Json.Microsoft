using System;

namespace CoreHal.Json.Tests.EndToEnd.SimpleEcommerceDomain
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderValue { get; set; }
        public string OrderNumber { get; set; }
    }
}