using System.Text;
using Microsoft.Extensions.Logging;
using Proxmox.Fqdn.Exporter.Adapters;
using Proxmox.Fqdn.Exporter.Adapters.Proxmox;

namespace Proxmox.Fqdn.Exporter.Services;

public class FqdnService
{
	private readonly ContainerProxmoxAdapter _containerProxmoxAdapter;
	private readonly ILogger<FqdnService> _logger;
	private readonly ProcessAdapter _processAdapter;
	private readonly VmProxmoxAdapter _vmProxmoxAdapter;


	public FqdnService(ProcessAdapter processAdapter, ILogger<FqdnService> logger, ContainerProxmoxAdapter containerProxmoxAdapter, VmProxmoxAdapter vmProxmoxAdapter)
	{
		_processAdapter = processAdapter;
		_logger = logger;
		_containerProxmoxAdapter = containerProxmoxAdapter;
		_vmProxmoxAdapter = vmProxmoxAdapter;
	}

	public string GetHostList(Data.Fqdn[] data)
	{
		var sb = new StringBuilder();

		foreach (var fqdn in data) sb.AppendLine($"{fqdn.Ip} {fqdn.Hostname}");

		return sb.ToString();
	}

	public async Task<Data.Fqdn[]> GetVmsFqdn()
	{
		_logger.LogInformation("Fetching VMs FQDN ...");

		var ids = await _vmProxmoxAdapter.GetAll();

		var tasks = ids.Select(async x => new Data.Fqdn(
			await _vmProxmoxAdapter.GetIp(x.Id), x.Name)
		);

		return await Task.WhenAll(tasks);
	}


	public async Task<Data.Fqdn> GetHostFqdn()
	{
		_logger.LogInformation("Fetching host FQDN...");


		var name = await _processAdapter.RunAsString(RunParameters.FromCommand("hostname"));
		var ip = await _processAdapter.RunAsString(RunParameters.FromCommand("hostname -i"));

		return new Data.Fqdn(ip.Trim(), name.Trim());
	}

	public async Task<Data.Fqdn[]> GetContainersFqdn()
	{
		_logger.LogInformation("Fetching Containers FQDN ...");

		var ids = await _containerProxmoxAdapter.GetAll();

		var tasks = ids.Select(async x => new Data.Fqdn(
			await _containerProxmoxAdapter.GetIp(x.Id), x.Name)
		);

		return await Task.WhenAll(tasks);
	}
}