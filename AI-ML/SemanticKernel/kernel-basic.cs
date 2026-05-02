// Semantic Kernel basics: build kernel, register native plugin, auto function calling.
//
// dotnet add package Microsoft.SemanticKernel

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public sealed class TimePlugin
{
    [KernelFunction("now"), Description("Get current UTC time as ISO 8601.")]
    public string Now() => DateTime.UtcNow.ToString("o");

    [KernelFunction("days_until"), Description("Days from today until a date (yyyy-MM-dd).")]
    public int DaysUntil([Description("Target date yyyy-MM-dd")] string date)
        => (DateOnly.Parse(date).ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).Days;
}

class Program
{
    static async Task Main()
    {
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-5", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
            .Build();

        kernel.Plugins.AddFromType<TimePlugin>("Time");

        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0,
        };

        var result = await kernel.InvokePromptAsync(
            "How many days until 2026-12-31?",
            new KernelArguments(settings));

        Console.WriteLine(result);
    }
}
