using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using Proxmox.Fqdn.Exporter.Abstractions.Technical;

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

	public async Task<Result<string>> RunAsString(RunParameters param)
	{
		_logger.LogDebug($"Running command: {param.Program} {param.Args} in {param.WorkingDirectory}");


		var error = "";
		var exitCode = 0;
		try
		{
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
			exitCode = process.ExitCode;

			var output = await process.StandardOutput.ReadToEndAsync();
			error = await process.StandardError.ReadToEndAsync();


			if (process.ExitCode != 0)
			{
				_logger.LogError("Command '{Program} {Args}' failed with exit code {ExitCode}: {Error}", param.Program, param.Args, process.ExitCode, error.Trim());
				return new Exception($"Command '{param.Program} {param.Args}' failed with exit code {process.ExitCode}: {error.Trim()}");
			}

			process.Dispose();

			return output;
		}
		catch (Exception e)
		{
			_logger.LogError("Command '{Program} {Args}' failed with exit code {ExitCode}: {Error}", param.Program, param.Args, exitCode, error.Trim());
			return new Exception($"Command '{param.Program} {param.Args}' failed with exit code {exitCode}: {error.Trim()}", e);
		}
	}

	public async Task<Result<T>> RunAsJson<T>(RunParameters param, IJsonTypeInfoResolver ctx)
	{
		var output = await RunAsString(param);

		if (!output.Success)
		{
			_logger.LogError("Failed to run command as JSON: {Error}", output.Error);
			return output.Error;
		}

		return _jsonAdapter.ParseIot<T>(output, ctx);
	}
}