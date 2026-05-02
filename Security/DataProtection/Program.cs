// ASP.NET Core Data Protection — multi-instance, prod-ready.
// Packages:
//   Microsoft.AspNetCore.DataProtection.AzureStorage
//   Microsoft.AspNetCore.DataProtection.AzureKeyVault
//   Azure.Identity
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
var credential = new DefaultAzureCredential();

builder.Services.AddDataProtection()
    .SetApplicationName("orders")
    .PersistKeysToAzureBlobStorage(
        new Uri(builder.Configuration["DataProtection:BlobUri"]!),
        credential)
    .ProtectKeysWithAzureKeyVault(
        new Uri(builder.Configuration["DataProtection:KeyId"]!),
        credential)
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();
app.MapGet("/", () => "OK");
app.Run();
