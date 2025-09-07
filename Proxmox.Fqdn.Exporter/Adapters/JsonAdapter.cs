using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Proxmox.Fqdn.Exporter.Abstractions.Technical;

namespace Proxmox.Fqdn.Exporter.Adapters;

/// <summary>
///     A class for parsing JSON data into specified types using a provided JSON type info resolver.
/// </summary>
public class JsonAdapter
{
	private readonly ILogger<JsonAdapter> _logger;

	public JsonAdapter(ILogger<JsonAdapter> logger)
	{
		_logger = logger;
	}


	public Result<T> ParseIot<T>(string json, IJsonTypeInfoResolver resolver)
	{
		var options = new JsonSerializerOptions
		{
			TypeInfoResolver = resolver,
			PropertyNameCaseInsensitive = true
		};

		try
		{
			return JsonSerializer.Deserialize<T>(json, options) ?? throw new JsonException($"Unable to deserialise {typeof(T).Name}");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to parse JSON data: {Json}", json);
			return e;
		}
	}
}