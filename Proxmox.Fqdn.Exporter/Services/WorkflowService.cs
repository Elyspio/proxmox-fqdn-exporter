using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proxmox.Fqdn.Exporter.Data;
using Proxmox.Fqdn.Exporter.Options;
using Proxmox.Fqdn.Exporter.Repositories;

namespace Proxmox.Fqdn.Exporter.Services;

public class WorkflowService
{
	private readonly FqdnService _fqdnService;
	private readonly PiholeService _piholeService;
	private readonly ILogger<WorkflowService> _logger;
	private readonly FqdnRepository _fqdnRepository;
	private readonly IOptions<AppConfig> _appConfig;

	public WorkflowService(FqdnService fqdnService, PiholeService piholeService, ILogger<WorkflowService> logger, FqdnRepository fqdnRepository, IOptions<AppConfig> appConfig)
	{
		_fqdnService = fqdnService;
		_piholeService = piholeService;
		_logger = logger;
		_fqdnRepository = fqdnRepository;
		_appConfig = appConfig;
	}

	public async Task Run()
	{
		var now = Stopwatch.GetTimestamp();

		var fqdn = await GetCurrentFqdn();


		var thresholdDate = DateTime.UtcNow.Add(-TimeSpan.FromMinutes(_appConfig.Value.FqdnRetentionMinutes));

		await _fqdnRepository.DeleteOlderThan(thresholdDate);

		var oldFqdn = (await _fqdnRepository.GetAll()).Cast<IFqdnWithTimestamp>().ToArray();

		var newFqdn = fqdn.Concat(oldFqdn).GroupBy(f => f.Hostname).Select(g => g.MaxBy(f => f.Hostname)).ToArray();

		await _fqdnRepository.DeleteOlderThan(DateTime.MaxValue);
		
		await _fqdnRepository.AddRange(newFqdn!);

		var hostList = _fqdnService.GetHostList(newFqdn!);

		await _piholeService.UpdateCustomFqdn(hostList);

		_logger.LogInformation("FQDN Exporter completed in {TotalSeconds} seconds.", Stopwatch.GetElapsedTime(now).TotalSeconds);
	}

	private async Task<FqdnWithTimestamp[]> GetCurrentFqdn()
	{
		// Placeholder for future implementation

		var hostFqdn = _fqdnService.GetHostFqdn();
		var vmsFqdn = _fqdnService.GetVmsFqdn();
		var containersFqdn = _fqdnService.GetContainersFqdn();

		await Task.WhenAll(hostFqdn, vmsFqdn, containersFqdn);

		Data.Fqdn[] arr = [hostFqdn.Result, ..vmsFqdn.Result, ..containersFqdn.Result];

		return arr.Select(fqdn => new FqdnWithTimestamp(fqdn.Ip, fqdn.Hostname, DateTime.Now)).ToArray();
	}
}