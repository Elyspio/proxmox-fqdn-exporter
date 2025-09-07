using Proxmox.Fqdn.Exporter.Abstractions.Technical;
using Proxmox.Fqdn.Exporter.Data;

namespace Proxmox.Fqdn.Exporter.Abstractions.Interfaces.Adapters;

public interface IProxmoxAdapter
{
	public const string RunningState = "running";


	Task<List<ProxmoxElement>> GetAll();

	Task<Result<string>> GetIp(short id);

	Task ReadFile(short id, string filepath);
	Task WriteFile(short id, string filepath, string content);

	Task SendCommand(short id, string command);
}