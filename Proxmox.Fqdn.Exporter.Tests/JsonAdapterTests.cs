using Microsoft.Extensions.Logging.Abstractions;
using Proxmox.Fqdn.Exporter.Adapters;
using Proxmox.Fqdn.Exporter.Data;

namespace Proxmox.Fqdn.Exporter.Tests;

public class JsonAdapterTests
{
	[Fact]
	public async Task ParseNetworkGetInterfacesResult()
	{
		var data = await File.ReadAllTextAsync("data.json");

		var sut = new JsonAdapter(new NullLogger<JsonAdapter>());

		var result = sut.ParseIot<NetworkGetInterfacesResult[]>(data, NetworkGetInterfacesContext.Default);

		Assert.True(result.Success);
		
		Assert.NotEmpty(result.Data!);
	}
}