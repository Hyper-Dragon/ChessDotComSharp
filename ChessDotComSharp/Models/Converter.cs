using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ChessDotComSharp.Models
{
    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new UnixDateTimeConverter(),
                new TolerantStringEnumConverter()
            },
        };
    }

    /// <summary>
    /// Chess.com adds game variants and time classes over time. The stock StringEnumConverter
    /// throws on any value it does not know, which used to lose a whole month of games, so an
    /// unrecognised value now deserialises to the enum's default member instead.
    /// </summary>
    internal class TolerantStringEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception ex) when (ex is JsonSerializationException or ArgumentException or FormatException)
            {
                return Nullable.GetUnderlyingType(objectType) is null
                    ? Activator.CreateInstance(objectType)
                    : null;
            }
        }
    }
}
