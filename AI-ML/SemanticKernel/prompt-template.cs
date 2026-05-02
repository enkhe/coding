// Prompt-as-template with handlebars and KernelArguments.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-5", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!)
    .Build();

const string template = """
    <message role="system">You translate English to {{language}}. Output the translation only.</message>
    <message role="user">{{input}}</message>
    """;

var fn = kernel.CreateFunctionFromPrompt(
    new PromptTemplateConfig(template)
    {
        TemplateFormat = HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
        InputVariables =
        {
            new() { Name = "language" },
            new() { Name = "input" },
        }
    },
    new HandlebarsPromptTemplateFactory());

var result = await kernel.InvokeAsync(fn, new KernelArguments
{
    ["language"] = "Mongolian",
    ["input"] = "The mountains are beautiful in winter.",
});

Console.WriteLine(result);
