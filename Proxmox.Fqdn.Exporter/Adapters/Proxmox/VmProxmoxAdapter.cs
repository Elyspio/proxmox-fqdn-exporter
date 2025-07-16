using Microsoft.Extensions.Logging;
using Proxmox.Fqdn.Exporter.Data;
using Proxmox.Fqdn.Exporter.Interfaces.Adapters;
using Proxmox.Fqdn.Exporter.Options;

namespace Proxmox.Fqdn.Exporter.Adapters.Proxmox;

public class VmProxmoxAdapter : IProxmoxAdapter
{
	private readonly ILogger<ContainerProxmoxAdapter> _logger;
	private readonly NetworkAdapter _networkAdapter;
	private readonly ProcessAdapter _processAdapter;

	public VmProxmoxAdapter(ILogger<ContainerProxmoxAdapter> logger, ProcessAdapter processAdapter, NetworkAdapter networkAdapter)
	{
		_logger = logger;
		_processAdapter = processAdapter;
		_networkAdapter = networkAdapter;
	}


	/// <inheritdoc />
	public async Task<List<ProxmoxElement>> GetAll()
	{
		var result = await _processAdapter.RunAsString(RunParameters.FromCommand("/usr/sbin/qm list | awk 'NR>1 {print $1, $2, $3}'"));


		var elements = new List<ProxmoxElement>();

		foreach (var line in result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
		{
			_logger.LogDebug("Processing line: {Line}", line);

			var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

			if (parts[2] == IProxmoxAdapter.RunningState) elements.Add(new ProxmoxElement(short.Parse(parts[0]), parts[1], ProxmoxElementType.Vm));
		}


		return elements;
	}

	/// <inheritdoc />
	public async Task<string> GetIp(short id)
	{
		var runParameters = RunParameters.FromCommand($"/usr/sbin/qm guest cmd {id} network-get-interfaces");

		var vmConfigs = await _processAdapter.RunAsJson<NetworkGetInterfacesResult[]>(runParameters, NetworkGetInterfacesContext.Default);


		return vmConfigs.SelectMany(x => x.Ips).First(f => f.Type == "ipv4" && _networkAdapter.IsInSubnet(f.Address)).Address;
	}

	/// <inheritdoc />
	public Task ReadFile(short id, string filepath)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task WriteFile(short id, string filepath, string content)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task SendCommand(short id, string command)
	{
		throw new NotImplementedException();
	}
}