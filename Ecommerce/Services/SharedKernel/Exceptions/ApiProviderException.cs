using System.Diagnostics;

namespace SharedKernel.Exceptions;

public sealed class ApiProviderException() : Exception(BuildMessageFromStack())
{
    private static string BuildMessageFromStack()
    {
        var trace = new StackTrace();
        var method = trace.GetFrame(2)?.GetMethod() ?? trace.GetFrame(1)?.GetMethod();
        var className = method?.DeclaringType?.Name ?? "<unknown>";
        var methodName = method?.Name ?? "<unknown>";
        return $"An error occurred while executing {className}.{methodName}.";
    }
}
