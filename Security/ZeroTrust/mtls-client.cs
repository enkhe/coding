// Outbound mTLS — client cert presented to a downstream service.
// Cert loaded from Key Vault via DefaultAzureCredential.
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ =>
{
    var kvUri = new Uri(builder.Configuration["KeyVault:Uri"]!);
    var credential = new DefaultAzureCredential();
    var certName = "payments-client";
    var certClient = new CertificateClient(kvUri, credential);
    var secretClient = new SecretClient(kvUri, credential);

    var cert = certClient.GetCertificate(certName).Value;
    var secret = secretClient.GetSecret(cert.SecretId.Segments[^2].TrimEnd('/')).Value;

    // For PKCS12 with no password
    return new X509Certificate2(Convert.FromBase64String(secret.Value), (string?)null,
        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
});

builder.Services.AddHttpClient("payments", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Payments:BaseUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler((sp) =>
{
    var clientCert = sp.GetRequiredService<X509Certificate2>();
    return new SocketsHttpHandler
    {
        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        {
            ClientCertificates = new X509Certificate2Collection { clientCert },
            // Trust pinning — replace with your provider's CA / fingerprint.
            RemoteCertificateValidationCallback = (sender, cert, chain, errs) =>
            {
                if (errs == System.Net.Security.SslPolicyErrors.None) return true;
                // Pin by fingerprint for self-signed providers.
                return cert is X509Certificate2 c2 &&
                    c2.Thumbprint == builder.Configuration["Payments:CertThumbprint"];
            }
        }
    };
});

var app = builder.Build();
app.Run();
