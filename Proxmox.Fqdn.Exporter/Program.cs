using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Proxmox.Fqdn.Exporter.Adapters;
using Proxmox.Fqdn.Exporter.Adapters.Proxmox;
using Proxmox.Fqdn.Exporter.Data;
using Proxmox.Fqdn.Exporter.Options;
using Proxmox.Fqdn.Exporter.Services;

var now = Stopwatch.GetTimestamp();

var builder = Host.CreateApplicationBuilder();

builder.Logging.AddSystemdConsole().SetMinimumLevel(LogLevel.Debug);

builder.Configuration.AddJsonFile("config.json", false, true).AddEnvironmentVariables();


builder.Services.Configure<AppConfig>(builder.Configuration);

builder.Services.AddSingleton<JsonAdapter>();
builder.Services.AddSingleton<NetworkAdapter>();
builder.Services.AddSingleton<ProcessAdapter>();

builder.Services.AddSingleton<ConfigService>();
builder.Services.AddSingleton<FqdnService>();
builder.Services.AddSingleton<PiholeService>();

builder.Services.AddSingleton<ContainerProxmoxAdapter>();
builder.Services.AddSingleton<VmProxmoxAdapter>();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

var configService = host.Services.GetRequiredService<ConfigService>();
configService.Verify();

var fqdnService = host.Services.GetRequiredService<FqdnService>();


var hostFqdn = fqdnService.GetHostFqdn();
var vmsFqdn = fqdnService.GetVmsFqdn();
var containersFqdn = fqdnService.GetContainersFqdn();

await Task.WhenAll(hostFqdn, vmsFqdn, containersFqdn);

Fqdn[] fqdn = [hostFqdn.Result, ..vmsFqdn.Result, ..containersFqdn.Result];

var hostList = fqdnService.GetHostList(fqdn);


var piholeService = host.Services.GetRequiredService<PiholeService>();
await piholeService.UpdateCustomFqdn(hostList);

logger.LogInformation("FQDN Exporter completed in {TotalSeconds} seconds.", Stopwatch.GetElapsedTime(now).TotalSeconds);