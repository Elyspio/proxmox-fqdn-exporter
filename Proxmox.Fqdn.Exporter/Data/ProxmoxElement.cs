using Proxmox.Fqdn.Exporter.Options;

namespace Proxmox.Fqdn.Exporter.Data;

/// <summary>
///     Represents a Proxmox VM or LXC.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
public record ProxmoxElement(short Id, string Name, ProxmoxElementType Type);