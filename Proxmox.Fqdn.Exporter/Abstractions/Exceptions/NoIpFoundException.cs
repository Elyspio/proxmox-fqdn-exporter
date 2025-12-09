using Proxmox.Fqdn.Exporter.Options;

namespace Proxmox.Fqdn.Exporter.Abstractions.Exceptions;

public class NoIpFoundException: Exception
{
	public NoIpFoundException(ProxmoxElementType type, int id) : base($"No IP found for {type} with ID {id}")
	{
	}
}