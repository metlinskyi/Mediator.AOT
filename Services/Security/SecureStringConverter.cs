using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Services.Security;

public class SecureStringConverter : JsonConverter<SecureString>
{
    public override SecureString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString() ?? string.Empty;
        var secureString = new SecureString();
        foreach (var c in value)
        {
            secureString.AppendChar(c);
        }
        secureString.MakeReadOnly();
        return secureString;
    }

    public override void Write(Utf8JsonWriter writer, SecureString value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}