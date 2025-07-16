using Microsoft.Extensions.Logging;
using Proxmox.Fqdn.Exporter.Data;
using Proxmox.Fqdn.Exporter.Interfaces.Adapters;
using Proxmox.Fqdn.Exporter.Options;

namespace Proxmox.Fqdn.Exporter.Adapters.Proxmox;

public class ContainerProxmoxAdapter : IProxmoxAdapter
{
	private readonly ILogger<ContainerProxmoxAdapter> _logger;
	private readonly NetworkAdapter _networkAdapter;
	private readonly ProcessAdapter _processAdapter;

	public ContainerProxmoxAdapter(ProcessAdapter processAdapter, ILogger<ContainerProxmoxAdapter> logger, NetworkAdapter networkAdapter)
	{
		_processAdapter = processAdapter;
		_logger = logger;
		_networkAdapter = networkAdapter;
	}

	/// <inheritdoc />
	public async Task<List<ProxmoxElement>> GetAll()
	{
		_logger.LogDebug("Fetching all Proxmox containers...");

		var result = await _processAdapter.RunAsString(RunParameters.FromCommand("/usr/sbin/pct list | awk 'NR>1 {print $1, $2, $3}'"));

		var elements = new List<ProxmoxElement>();

		foreach (var line in result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

			if (parts[1] == IProxmoxAdapter.RunningState) elements.Add(new ProxmoxElement(short.Parse(parts[0]), parts[2], ProxmoxElementType.Container));
		}


		return elements;
	}

	/// <inheritdoc />
	public async Task<string> GetIp(short id)
	{
		_logger.LogDebug("Fetching IP for container {Id}", id);

		var param = RunParameters.FromCommand($"/usr/sbin/pct exec {id} -- hostname -I");

		var result = await _processAdapter.RunAsString(param);

		var addresses = result.Split(" ").Select(ip => ip.Trim());

		return addresses.First(addr => _networkAdapter.IsInSubnet(addr));
	}

	/// <inheritdoc />
	public Task ReadFile(short id, string filepath)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public async Task WriteFile(short id, string filepath, string content)
	{
		_logger.LogDebug("Writing file to container {Id} at {FilePath}", id, filepath);

		var tempPath = Path.GetTempFileName() + Guid.NewGuid();

		try
		{
			await File.WriteAllTextAsync(tempPath, content);

			var param = RunParameters.FromCommand($"/usr/sbin/pct push {id} '{tempPath}' '{filepath}'");

			await _processAdapter.RunAsString(param);
		}
		finally
		{
			if (File.Exists(tempPath))
			{
				File.Delete(tempPath);
			}
		}
	}

	/// <inheritdoc />
	public async Task SendCommand(short id, string command)
	{
		_logger.LogDebug("Sending command to container {Id}: {Command}", id, command);

		var param = RunParameters.FromCommand($"/usr/sbin/pct exec {id} -- {command}");

		await _processAdapter.RunAsString(param);
	}
}