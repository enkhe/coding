// ChatCompletionAgent — multi-turn agent with tools and instructions.
//
// dotnet add package Microsoft.SemanticKernel.Agents.Core

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public sealed class CalculatorPlugin
{
    [KernelFunction, Description("Add two numbers.")]
    public double Add(double a, double b) => a + b;

    [KernelFunction, Description("Multiply two numbers.")]
    public double Multiply(double a, double b) => a * b;
}

class Program
{
    static async Task Main()
    {
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-5", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
            .Build();
        kernel.Plugins.AddFromType<CalculatorPlugin>();

        ChatCompletionAgent agent = new()
        {
            Name = "MathTutor",
            Instructions = "You teach math step-by-step. Always use tools for arithmetic.",
            Kernel = kernel,
            Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                Temperature = 0,
            }),
        };

        ChatHistory history = new();
        history.AddUserMessage("Compute (3 + 4) * 5 step by step.");

        await foreach (var msg in agent.InvokeAsync(history))
            Console.WriteLine($"{msg.Message.Role}: {msg.Message.Content}");
    }
}
