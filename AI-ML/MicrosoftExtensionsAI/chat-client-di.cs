// ASP.NET Core 10 registration of IChatClient + IEmbeddingGenerator with full pipeline.
//
// dotnet add package Microsoft.Extensions.AI
// dotnet add package Microsoft.Extensions.AI.OpenAI
// dotnet add package OpenTelemetry.Extensions.Hosting

using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var openAi = new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]);

builder.Services
    .AddChatClient(b => b
        .UseLogging()
        .UseOpenTelemetry(sourceName: "myapp.ai", configure: o => o.EnableSensitiveData = false)
        .UseDistributedCache()
        .UseFunctionInvocation()
        .Use(openAi.AsChatClient("gpt-5")));

builder.Services
    .AddEmbeddingGenerator(b => b
        .UseOpenTelemetry(sourceName: "myapp.embeddings")
        .Use(openAi.AsEmbeddingGenerator("text-embedding-3-small")));

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.MapPost("/chat", async (IChatClient chat, string prompt) =>
{
    var resp = await chat.GetResponseAsync(prompt,
        new ChatOptions { Temperature = 0.2f, MaxOutputTokens = 512 });
    return Results.Ok(new { text = resp.Text, usage = resp.Usage });
});

app.Run();
