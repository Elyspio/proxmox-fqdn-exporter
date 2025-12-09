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
    
    /// <summary>
    /// Number of minutes to retain FQDN records in the database even if they are no longer present.
    /// </summary>
    public required int FqdnRetentionMinutes { get; set; }
}