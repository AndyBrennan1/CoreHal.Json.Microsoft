using CoreHal.Graph;
using CoreHal.PropertyNaming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreHal.Json
{
    public class HalGraphCollectionJsonConvertor : JsonConverter<List<IHalGraph>>
    {
        private readonly IPropertyNameConvention propertyNameConvention;

        public HalGraphCollectionJsonConvertor(IPropertyNameConvention propertyNameConvention)
        {
            this.propertyNameConvention = propertyNameConvention;
        }

        public override List<IHalGraph> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, List<IHalGraph> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (HalGraph halGraph in value)
            {
                writer.WriteStartObject();

                foreach (DictionaryEntry entry in halGraph)
                {
                    if (IsNotEmptyReservedSection(entry))
                    {
                        if (entry.Key.ToString() == "_links")
                        {

                            writer.WritePropertyName(ApplyConvention(entry.Key.ToString()));
                            writer.WriteStartObject();

                            var x = (Dictionary<string, object>)entry.Value;

                            foreach (KeyValuePair<string, object> kvp in x)
                            {
                                writer.WritePropertyName(ApplyConvention(kvp.Key.ToString()));
                                JsonSerializer.Serialize(writer, kvp.Value, options);
                            }
                            writer.WriteEndObject();
                        }
                        else if (entry.Key.ToString() == "_embedded")
                        {
                            writer.WritePropertyName(ApplyConvention(entry.Key.ToString()));

                            if (entry.Value.GetType() == typeof(Dictionary<string, object>))
                            {
                                foreach (var y in (Dictionary<string, object>)entry.Value)
                                {
                                    writer.WriteStartObject();
                                    writer.WritePropertyName(ApplyConvention(y.Key.ToString()));

                                    if (y.Value.GetType() == typeof(List<IHalGraph>))
                                    {
                                        var x = new HalGraphCollectionJsonConvertor(propertyNameConvention);
                                        x.Write(writer, (List<IHalGraph>)y.Value, options);
                                    }
                                    else
                                    {
                                        var x = new HalGraphJsonConvertor(propertyNameConvention);
                                        x.Write(writer, (HalGraph)y.Value, options);
                                    }



                                    //JsonSerializer.Serialize(writer, y.Value, options);


                                    writer.WriteEndObject();
                                }
                            }
                        }
                        else
                        {
                            writer.WritePropertyName(ApplyConvention(entry.Key.ToString()));
                            JsonSerializer.Serialize(writer, entry.Value, options);
                        }
                    }
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private string ApplyConvention(string propertyName)
        {
            if (propertyName != "_links" && propertyName != "_embedded")
            {
                return propertyNameConvention.Apply(propertyName);
            }
            else
            {
                return propertyName;
            }
        }

        private static bool IsNotEmptyReservedSection(DictionaryEntry entry)
        {
            return !(IsEmbeddedCollectionAndNullOrEmpty(entry) || IsLinkCollectionAndNullOrEmpty(entry));
        }

        private static bool IsEmbeddedCollectionAndNullOrEmpty(DictionaryEntry entry)
        {
            if (entry.Key.ToString() == "_embedded")
            {
                if (entry.Value == null || !((IDictionary<string, object>)entry.Value).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private static bool IsLinkCollectionAndNullOrEmpty(DictionaryEntry entry)
        {
            if (entry.Key.ToString() == "_links")
            {
                if (entry.Value == null || !((IDictionary<string, object>)entry.Value).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
