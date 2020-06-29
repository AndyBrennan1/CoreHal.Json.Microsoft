using ApprovalTests;
using ApprovalTests.Reporters;
using CoreHal.Graph;
using CoreHal.Json.Tests.Fixtures;
using CoreHal.PropertyNaming;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace CoreHal.Json.Tests.HalGraphCollection
{
    [Collection("HAL Graph Collection - Json Convertor Tests")]
    public class HalGraphCollectionJsonConvertorTests
    {
        [UseReporter(typeof(VisualStudioReporter))]
        [Fact]
        public void WritingHalGraphCollection_NoEmbeddedOrLinks_WritesPropertyButOmitsEmbeddedItemsAndLinks()
        {
            var propertyNameConvention = new Mock<IPropertyNameConvention>();
            propertyNameConvention.Setup(convention => convention.Apply("StringProperty")).Returns("string-Property");

            List<OnePropertyClass> OnePropertyClass = GetOnePropertyClassModelCollection();

            var halGraphCollection = new List<IHalGraph>();

            foreach (var model in OnePropertyClass)
            {
                halGraphCollection.Add(new HalGraph(model));
            }

            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new HalGraphCollectionJsonConvertor(propertyNameConvention.Object) },
                WriteIndented = true
            };

            var responseJson = JsonSerializer.Serialize(halGraphCollection, typeof(List<IHalGraph>), serializerOptions);

            Approvals.Verify(responseJson);
        }

        [UseReporter(typeof(VisualStudioReporter))]
        [Fact]
        public void WritingHalGraphCollection_EachWithOneLink_WritesPropertyAndAddsLink()
        {
            var propertyNameConvention = new Mock<IPropertyNameConvention>();
            propertyNameConvention.Setup(convention => convention.Apply("StringProperty")).Returns("string-Property");
            propertyNameConvention.Setup(convention => convention.Apply("TEST")).Returns("test");
            propertyNameConvention.Setup(convention => convention.Apply("href")).Returns("href");

            List<OnePropertyClass> OnePropertyClass = GetOnePropertyClassModelCollection();

            var halGraphCollection = new List<IHalGraph>();

            foreach (var model in OnePropertyClass)
            {
                halGraphCollection.Add(
                    new HalGraph(model).AddLink("TEST", new Link("/api/something")));
            }

            var serializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new HalGraphCollectionJsonConvertor(propertyNameConvention.Object),
                    new LinkJsonConvertor(propertyNameConvention.Object)
                },
                WriteIndented = true
            };

            var responseJson = JsonSerializer.Serialize(halGraphCollection, typeof(List<IHalGraph>), serializerOptions);

            Approvals.Verify(responseJson);
        }

        [UseReporter(typeof(DiffReporter))]
        [Fact]
        public void WritingHalGraphCollection_EachWithOneEmbeddedItem_WritesPropertyAndAddsEmbeddedItem()
        {
            var propertyNameConvention = new Mock<IPropertyNameConvention>();
            propertyNameConvention.Setup(convention => convention.Apply("StringProperty")).Returns("string-Property");
            propertyNameConvention.Setup(convention => convention.Apply("IntegerProperty")).Returns("integer-Property");
            propertyNameConvention.Setup(convention => convention.Apply("TEST")).Returns("test");
            propertyNameConvention.Setup(convention => convention.Apply("href")).Returns("href");

            List<OnePropertyClass> topLevelModels = GetOnePropertyClassModelCollection();
            var halGraphCollection = new List<IHalGraph>();

            var modelToEmbedd = new OneIntegerPropertyClass { IntegerProperty = 999 };
            var halGraphToEmbed = new HalGraph(modelToEmbedd);

            foreach (var model in topLevelModels)
            {
                halGraphCollection.Add(
                    new HalGraph(model).AddEmbeddedItem("TEST", halGraphToEmbed));
            }

            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new HalGraphCollectionJsonConvertor(propertyNameConvention.Object) },
                WriteIndented = true
            };

            var responseJson = JsonSerializer.Serialize(halGraphCollection, typeof(List<IHalGraph>), serializerOptions);


            Approvals.Verify(responseJson);
        }

        [UseReporter(typeof(VisualStudioReporter))]
        [Fact]
        public void WritingHalGraphCollection_EachWithEmbeddedSetWith2Items_WritesPropertyAndAddsEmbeddedItems()
        {
            var propertyNameConvention = new Mock<IPropertyNameConvention>();
            propertyNameConvention.Setup(c => c.Apply("StringProperty")).Returns("string-Property");
            propertyNameConvention.Setup(c => c.Apply("TEST")).Returns("test");
            propertyNameConvention.Setup(c => c.Apply("IntegerProperty")).Returns("integer-Property");

            List<OnePropertyClass> topLevelModels = GetOnePropertyClassModelCollection();
            var halGraphCollection = new List<IHalGraph>();

            var modelToEmbedd1 = new OneIntegerPropertyClass { IntegerProperty = 111 };
            var modelToEmbedd2 = new OneIntegerPropertyClass { IntegerProperty = 222 };
            var halGraphToEmbed1 = new HalGraph(modelToEmbedd1);
            var halGraphToEmbed2 = new HalGraph(modelToEmbedd2);

            foreach (var model in topLevelModels)
            {
                halGraphCollection.Add(
                    new HalGraph(model).AddEmbeddedItemCollection(
                        "TEST", new List<HalGraph> { halGraphToEmbed1, halGraphToEmbed2 }));
            }

            var serializerOptions = new JsonSerializerOptions
            {
                Converters = {
                    new HalGraphCollectionJsonConvertor(propertyNameConvention.Object),
                    new LinkJsonConvertor(propertyNameConvention.Object)
                },
                WriteIndented = true
            };

            var responseJson = JsonSerializer.Serialize(halGraphCollection, typeof(List<IHalGraph>), serializerOptions);

            Approvals.Verify(responseJson);
        }

        private static List<OnePropertyClass> GetOnePropertyClassModelCollection()
        {
            return new List<OnePropertyClass>
            {
                new OnePropertyClass {
                    StringProperty = "String Property 1"
                },
                new OnePropertyClass {
                    StringProperty = "String Property 2"
                },
                new OnePropertyClass {
                    StringProperty = "String Property 3"
                }
            };
        }
    }
}