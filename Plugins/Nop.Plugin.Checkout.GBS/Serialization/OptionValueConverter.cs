using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Checkout.GBS.Models;

namespace Nop.Plugin.Checkout.GBS.Models
{
    internal class OptionValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var optVal = new IdOptionValue();
                    foreach (var property in (JObject)serializer.Deserialize(reader))
                    {
                        switch (property.Key)
                        {
                            case "id":
                                optVal.Id = property.Value.Value<int>();
                                break;

                            case "preselected":
                                optVal.Preselected = property.Value.Value<bool>();
                                break;

                            case "title":
                                optVal.Title = property.Value.Value<string>();
                                break;
                            case "price":
                                optVal.Price = property.Value.Value<float>();
                                break;
                        }
                    }
                    return optVal;
                case JsonToken.String:
                    return new StringOptionValue { StringValue = (string)reader.Value };
            }

            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(OptionValue).IsAssignableFrom(objectType);
        }
    }
}
