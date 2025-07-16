namespace Proxmox.Fqdn.Exporter.Options;

/// <summary>
///     Types of Proxmox elements.
/// </summary>
public enum ProxmoxElementType
{
	/// <summary>
	///     Proxmox LXC container.
	/// </summary>
	Container,

	/// <summary>
	///     Proxmox virtual machine.
	/// </summary>
	Vm
}