using Microsoft.Extensions.Options;
using Proxmox.Fqdn.Exporter.Options;

namespace Proxmox.Fqdn.Exporter.Services;

public class ConfigService
{
	private readonly IOptionsMonitor<AppConfig> _config;

	public ConfigService(IOptionsMonitor<AppConfig> config)
	{
		_config = config;
	}

	public void Verify()
	{
		Verify(_config.CurrentValue);
		_config.OnChange(Verify);
	}


	private void Verify(AppConfig config)
	{
		if (config.SubnetsFilter.Length == 0) throw new ArgumentException("SubnetsFilter is required in the configuration.");

		switch (config.Export)
		{
			case null:
				throw new ArgumentException("Export configuration is required.");
			case { Console: false, Pihole: null }:
				throw new ArgumentException("Atleast one export method must be enabled: Console or Pihole.");
		}
	}
}