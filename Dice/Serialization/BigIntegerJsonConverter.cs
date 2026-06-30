using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dice.Serialization;

/// <summary>
/// System.Text.Json has no built-in support for <see cref="BigInteger"/>. This converter
/// reads and writes it as a raw JSON number so exact integer counts of any size survive
/// serialization without precision loss or quoting.
/// </summary>
public class BigIntegerJsonConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        return BigInteger.Parse(doc.RootElement.GetRawText(), CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString(CultureInfo.InvariantCulture));
    }
}
