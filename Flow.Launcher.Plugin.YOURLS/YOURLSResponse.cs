using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.YOURLS
{
    internal class NumberOrStringConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt32(),
                JsonTokenType.String when int.TryParse(reader.GetString(), out int result) => result,
                _ => throw new JsonException("Invalid number format")
            };
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    internal class YOURLSResponse
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
        [JsonConverter(typeof(NumberOrStringConverter))]
        public string statusCode { get; set; }
        public string title { get; set; }
        public string shorturl { get; set; }
        public UrlInformation url { get; set; }
    }

    internal class UrlInformation
    {
        public string keyword { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string date { get; set; }
        public string ip { get; set; }
        public int clicks { get; set; }
    }
}
