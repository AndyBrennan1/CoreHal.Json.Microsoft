using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Text.Json;
using ApprovalTests.Reporters;
using ApprovalTests;
using CoreHal.Graph;
using CoreHal.PropertyNaming;
using CoreHal.Json.Tests.EndToEnd.SimpleEcommerceDomain;

namespace CoreHal.Json.Tests.EndToEnd
{
    [UseReporter(typeof(VisualStudioReporter))]
    [Collection("End to End Examples")]
    public class Examples
    {
        public HalGraph IHalGraph { get; private set; }

        [Fact]
        public void Example1()
        {
            var model = new Order
            {
                CustomerName = "A B Systems",
                OrderDate = new DateTime(2020, 6, 23),
                OrderId = Guid.Parse("AA957846-2448-42DC-A1CB-90E9E399F57C"),
                OrderNumber = "ORDER123",
                OrderValue = 6.5M
            };

            var customer = new Customer
            {
                CustomerId = Guid.Parse("ACE2A06E-5FE5-44DE-BDFC-AAFC9C507DCC"),
                CompanyName = "A-B-Systems",
                PrimaryContact = "Andy Brennan",
                PrimaryPhoneNumber = "+44787642628292",
                Sector = new Sector
                {
                    SectorId = Guid.Parse("030A522A-EF75-44F3-AF61-62FA8FA655A9"),
                    Name = "NuGet Making"
                }
            };

            var orderLines = new List<OrderLine>
            {
                new OrderLine {
                    OrderLineId = Guid.Parse("999cc766-5370-4d66-9aa8-6ae02787762c"),
                    ProductName = "Apple",
                    SKU = "APP123",
                    LineValue = 1.0M,
                    Quantity = 2
                },
                new OrderLine {
                    OrderLineId = Guid.Parse("950c1b91-38a3-413c-9d00-b886ea435943"),
                    ProductName = "Orange",
                    SKU = "ORA555",
                    LineValue = 1.5M,
                    Quantity = 2
                },
                new OrderLine {
                    OrderLineId = Guid.Parse("e258854b-9a6b-4ff0-86c2-a8fe2589d276"),
                    ProductName = "Pear",
                    SKU = "PER88",
                    LineValue = 3.0M,
                    Quantity = 6
                }
            };

            IHalGraph customerGraph =
                new HalGraph(customer)
                .AddLink("self", new Link("/api/customers/A-B-Systems"));

            IHalGraph trySingleOrderLine =
                new HalGraph(orderLines[0])
                .AddLink("order", new Link("/api/orders/ORDER123"))
                .AddLink("parent", new Link("/api/orders/ORDER123/order-lines"))
                .AddLink("self", new Link($"/api/orders/ORDER123/order-lines/{orderLines[0].OrderLineId}"));

            IEnumerable<IHalGraph> orderlineGraphs = orderLines.Select(
                orderLine => new HalGraph(orderLine)
                .AddLink("order", new Link("/api/orders/ORDER123"))
                .AddLink("parent", new Link("/api/orders/ORDER123/order-lines"))
                .AddLink("self", new Link($"/api/orders/ORDER123/order-lines/{orderLine.OrderLineId}")));

            IHalGraph orderGraph =
                new HalGraph(model)
                .AddCurie(new Curie("ord", "/api/orders/{order-number}"))
                .AddLink("self", new Link("/api/orders/ORDER123"))
                .AddEmbeddedItem("customer", customerGraph)
                .AddEmbeddedItem("singular-order-line", trySingleOrderLine)
                .AddEmbeddedItemCollection("order-lines", orderlineGraphs);

            IPropertyNameConvention propertyNameConvention = new CamelDashNameConvention();

            var serializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new HalGraphJsonConvertor(propertyNameConvention),
                    new HalGraphCollectionJsonConvertor(propertyNameConvention),
                    new LinkJsonConvertor(propertyNameConvention),
                    new CurieLinkJsonConvertor(propertyNameConvention)
                },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var responseJson = JsonSerializer.Serialize(orderGraph, typeof(HalGraph), serializerOptions);

            Approvals.Verify(responseJson);
        }
    }
}
