namespace Proxmox.Fqdn.Exporter.Options;

/// <summary>
///     Represents the main application configuration.
/// </summary>
public class AppConfig
{
    /// <summary>
    ///     Subnet filter string used to filter network interfaces.
    /// </summary>
    public required string[] SubnetsFilter { get; set; }

    /// <summary>
    ///     Export configuration options.
    /// </summary>
    public required Export Export { get; set; }
}