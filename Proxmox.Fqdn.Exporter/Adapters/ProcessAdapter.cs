using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;

namespace Proxmox.Fqdn.Exporter.Adapters;

public record RunParameters(string Program, string Args = "", string WorkingDirectory = "/")
{
	public static RunParameters FromCommand(string command, string workingDirectory = "/")
	{
		var parts = command.Split(' ');

		return new RunParameters(parts[0], string.Join(' ', parts[1..]), workingDirectory);
	}
}

public class ProcessAdapter
{
	private readonly JsonAdapter _jsonAdapter;
	private readonly ILogger<ProcessAdapter> _logger;

	public ProcessAdapter(JsonAdapter jsonAdapter, ILogger<ProcessAdapter> logger)
	{
		_jsonAdapter = jsonAdapter;
		_logger = logger;
	}

	public async Task<string> RunAsString(RunParameters param)
	{
		_logger.LogDebug($"Running command: {param.Program} {param.Args} in {param.WorkingDirectory}");

		var processStartInfo = new ProcessStartInfo
		{
			FileName = "/bin/sh",
			Arguments = $"-c \"{param.Program} {param.Args}\"",
			UseShellExecute = false,
			CreateNoWindow = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			WorkingDirectory = param.WorkingDirectory
		};

		var process = new Process { StartInfo = processStartInfo };

		process.Start();

		await process.WaitForExitAsync();

		var output = await process.StandardOutput.ReadToEndAsync();
		var error = await process.StandardError.ReadToEndAsync();

		if (process.ExitCode != 0) throw new Exception($"Command failed with exit code {process.ExitCode}: {error}");

		process.Dispose();

		return output;
	}

	public async Task<T> RunAsJson<T>(RunParameters param, IJsonTypeInfoResolver ctx)
	{
		var output = await RunAsString(param);

		return _jsonAdapter.ParseIot<T>(output, ctx);
	}
}