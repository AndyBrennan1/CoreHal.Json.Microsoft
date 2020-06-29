using CoreHal.Graph;
using CoreHal.PropertyNaming;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreHal.Json
{
    public class CurieLinkJsonConvertor : JsonConverter<CurieLink>
    {
        private readonly IPropertyNameConvention propertyNameConvention;

        public CurieLinkJsonConvertor(IPropertyNameConvention propertyNameConvention)
        {
            this.propertyNameConvention = propertyNameConvention;
        }

        public override CurieLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, CurieLink value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(propertyNameConvention.Apply("name"), value.Name);
            writer.WriteString(propertyNameConvention.Apply("href"), value.Href.ToString());
            writer.WriteBoolean(propertyNameConvention.Apply("templated"), true);
            writer.WriteEndObject();
        }
    }
}