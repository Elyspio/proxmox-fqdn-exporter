﻿using System.Net;
using Microsoft.Extensions.Options;
using Proxmox.Fqdn.Exporter.Options;

namespace Proxmox.Fqdn.Exporter.Adapters;

public class NetworkAdapter
{
	private readonly IOptionsMonitor<AppConfig> _options;

	public NetworkAdapter(IOptionsMonitor<AppConfig> options)
	{
		_options = options;
	}

	/// <summary>
	///     Checks if the given IP address is within the configured subnet.
	///     See <see cref="AppConfig.SubnetFilter" /> for the expected value format, which should be in CIDR notation
	/// </summary>
	/// <param name="ip"></param>
	/// <returns></returns>
	public bool IsInSubnet(string ip)
	{
		var parts = _options.CurrentValue.SubnetFilter.Split('/');
		var networkBytes = IPAddress.Parse(parts[0]).GetAddressBytes().Reverse().ToArray();
		var ipBytes = IPAddress.Parse(ip).GetAddressBytes().Reverse().ToArray();
		var prefix = int.Parse(parts[1]);

		var mask = prefix == 0
			? 0u
			: 0xFFFFFFFFu << (32 - prefix);

		var net = BitConverter.ToUInt32(networkBytes, 0);
		var addr = BitConverter.ToUInt32(ipBytes, 0);

		return (addr & mask) == (net & mask);
	}
}