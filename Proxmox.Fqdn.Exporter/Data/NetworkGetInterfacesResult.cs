using System.Text.Json.Serialization;

namespace Proxmox.Fqdn.Exporter.Data;

public class NetworkGetInterfacesResult
{
	[JsonPropertyName("hardware-address")] public required string HardwareAddress { get; set; }

	[JsonPropertyName("ip-addresses")] public required List<IpAddress> Ips { get; set; }

	[JsonPropertyName("name")] public required string Name { get; set; }

	[JsonPropertyName("statistics")] public required Statistics Statistics { get; set; }
}

public class IpAddress
{
	[JsonPropertyName("ip-address")] public required string Address { get; set; }

	[JsonPropertyName("ip-address-type")] public required string Type { get; set; }

	[JsonPropertyName("prefix")] public long Prefix { get; set; }
}

public class Statistics
{
	[JsonPropertyName("rx-bytes")] public long RxBytes { get; set; }

	[JsonPropertyName("rx-dropped")] public long RxDropped { get; set; }

	[JsonPropertyName("rx-errs")] public long RxErrs { get; set; }

	[JsonPropertyName("rx-packets")] public long RxPackets { get; set; }

	[JsonPropertyName("tx-bytes")] public long TxBytes { get; set; }

	[JsonPropertyName("tx-dropped")] public long TxDropped { get; set; }

	[JsonPropertyName("tx-errs")] public long TxErrs { get; set; }

	[JsonPropertyName("tx-packets")] public long TxPackets { get; set; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(IpAddress))]
[JsonSerializable(typeof(Statistics))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(NetworkGetInterfacesResult[]))]
public partial class NetworkGetInterfacesContext : JsonSerializerContext;