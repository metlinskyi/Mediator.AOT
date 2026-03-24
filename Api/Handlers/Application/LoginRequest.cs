using System.Security;
using System.Text.Json.Serialization;
using Api.Services.Security;

namespace Api.Handlers.Application;

public record LoginRequest(
	string Username,
	[property: JsonConverter(typeof(SecureStringConverter))] SecureString Password);
