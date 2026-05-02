using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace Testing.EndToEnd;

// Page Object: hides selectors so tests read like user actions.
public sealed class LoginPage(IPage page)
{
    public Task GotoAsync() => page.GotoAsync("https://app.local/login");

    public async Task LoginAsync(string email, string password)
    {
        await page.GetByLabel("Email").FillAsync(email);
        await page.GetByLabel("Password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();
    }

    public ILocator ErrorBanner() => page.GetByRole(AriaRole.Alert);
    public ILocator WelcomeHeading() => page.GetByRole(AriaRole.Heading, new() { Name = "Welcome" });
}

public sealed class LoginTests : PageTest
{
    [Fact]
    public async Task ValidCredentials_Should_NavigateToDashboard()
    {
        var login = new LoginPage(Page);

        await login.GotoAsync();
        await login.LoginAsync("alice@example.com", "correct-horse");

        await Expect(login.WelcomeHeading()).ToBeVisibleAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*/dashboard"));
    }

    [Fact]
    public async Task InvalidCredentials_Should_ShowErrorBanner()
    {
        var login = new LoginPage(Page);

        await login.GotoAsync();
        await login.LoginAsync("alice@example.com", "wrong");

        await Expect(login.ErrorBanner()).ToContainTextAsync("Invalid email or password");
    }
}
