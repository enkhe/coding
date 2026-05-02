// Function/tool calling loop with Microsoft.Extensions.AI.
// Provider-agnostic: works against OpenAI, Azure OpenAI, Anthropic, Ollama.
//
// dotnet add package Microsoft.Extensions.AI
// dotnet add package Microsoft.Extensions.AI.OpenAI

using Microsoft.Extensions.AI;
using OpenAI;

[System.ComponentModel.Description("Get current weather for a city.")]
static string GetWeather(
    [System.ComponentModel.Description("City name")] string city)
    => $"It is 22C and sunny in {city}.";

var openAi = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
IChatClient client = openAi.AsChatClient("gpt-5")
    .AsBuilder()
    .UseFunctionInvocation() // executes tool calls automatically; loops until done
    .Build();

var tools = new List<AITool> { AIFunctionFactory.Create(GetWeather) };

var response = await client.GetResponseAsync(
    "What is the weather in Tokyo?",
    new ChatOptions { Tools = tools, Temperature = 0f });

Console.WriteLine(response.Text);
// Inspect tool calls
foreach (var msg in response.Messages)
foreach (var content in msg.Contents.OfType<FunctionCallContent>())
    Console.WriteLine($"called: {content.Name}({content.Arguments})");
