using System.Diagnostics.CodeAnalysis;

namespace Proxmox.Fqdn.Exporter.Abstractions.Technical;

public class Result<T>
{
	[MemberNotNullWhen(true, nameof(Data))]
	[MemberNotNullWhen(false, nameof(Error))]
	public bool Success { get; set; }

	public T? Data { get; set; }

	public Exception? Error { get; set; }

	private Result(T? data, Exception? error = null)
	{
		if (error is not null)
		{
			Success = false;
			Error = error;
		}
		else
		{
			Success = true;
			Data = data;
		}
	}

	public static implicit operator Result<T>(T data) => new(data);
	public static implicit operator Result<T>(Exception err) => new(default, err);
	public static implicit operator T (Result<T> result)
	{
		return !result.Success ? throw new InvalidOperationException("Cannot get Data from a failed Result.", result.Error) : result.Data!;
	}
}

public class Result
{
	public bool Success { get; set; }

	[MemberNotNullWhen(false, nameof(Success))]
	public Exception? Error { get; set; }

	public Result(Exception? error = null)
	{
		if (error is not null)
		{
			Success = false;
			Error = error;
		}
		else
		{
			Success = true;
		}
	}

	public static implicit operator Result(Exception err) => new(err);
}