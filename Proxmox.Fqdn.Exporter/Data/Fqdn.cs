namespace Proxmox.Fqdn.Exporter.Data;


public interface IFqdnWithTimestamp
{
	string Ip { get; }
	string Hostname { get; }
	DateTime UpdatedAt { get; }
}

public record Fqdn(string Ip, string Hostname);

public record FqdnWithTimestamp(string Ip, string Hostname, DateTime UpdatedAt): IFqdnWithTimestamp;

public record FqdnModel(string Ip, string Hostname, DateTime UpdatedAt): IFqdnWithTimestamp;