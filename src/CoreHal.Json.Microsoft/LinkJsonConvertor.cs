using CoreHal.Graph;
using CoreHal.PropertyNaming;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreHal.Json
{
    public class LinkJsonConvertor : JsonConverter<Link>
    {
        private readonly IPropertyNameConvention propertyNameConvention;

        public LinkJsonConvertor(IPropertyNameConvention propertyNameConvention)
        {
            this.propertyNameConvention = propertyNameConvention;
        }

        public override Link Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Link value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(propertyNameConvention.Apply("href"), value.Href.ToString());

            if (!string.IsNullOrEmpty(value.Title))
            {
                writer.WriteString(propertyNameConvention.Apply("title"), value.Title);
            }

            if (value.Templated == true)
            {
                writer.WriteString(propertyNameConvention.Apply("templated"), true.ToString());
            }

            writer.WriteEndObject();
        }
    }
}