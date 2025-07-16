namespace Proxmox.Fqdn.Exporter.Options;

/// <summary>
///     Configuration for exporting to Pihole.
/// </summary>
public class Pihole
{
	/// <summary>
	///     Pihole instance identifier.
	/// </summary>
	public short Id { get; set; }

	/// <summary>
	///     Path to the Pihole list file.
	/// </summary>
	public string ListFilePath { get; set; } = null!;

	/// <summary>
	///     Path to the Pihole executable.
	/// </summary>
	public string ExecutablePath { get; set; } = null!;

	/// <summary>
	///     Type of Proxmox element (Container or VM).
	/// </summary>
	public ProxmoxElementType Type { get; set; }
}