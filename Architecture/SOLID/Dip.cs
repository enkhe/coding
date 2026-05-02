// Dependency Inversion Principle
// High-level modules and low-level modules both depend on abstractions.

namespace Architecture.SOLID.Dip;

// BAD: high-level policy ("OrderProcessor") binds to a concrete logger and SMTP client.
public sealed class FileLogger_Bad
{
    public void Log(string m) => File.AppendAllText("log.txt", m + "\n");
}

public sealed class OrderProcessor_Bad
{
    private readonly FileLogger_Bad _log = new();
    public void Process(Guid orderId) => _log.Log($"Processed {orderId}");
}

// GOOD: depend on an abstraction; choose the implementation at the composition root.
public interface ILogger { void Log(string message); }

public sealed class FileLogger : ILogger
{
    public void Log(string m) => File.AppendAllText("log.txt", m + "\n");
}

public sealed class OrderProcessor(ILogger logger)
{
    public void Process(Guid orderId) => logger.Log($"Processed {orderId}");
}

// Composition root (e.g. Program.cs):
// services.AddSingleton<ILogger, FileLogger>();
// services.AddScoped<OrderProcessor>();
