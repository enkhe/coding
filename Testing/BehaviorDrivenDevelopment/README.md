# Behavior-Driven Development (BDD)

> Tests as conversations. **Given / When / Then** scenarios written in business language.

## Tooling (.NET 2026)

- **SpecFlow** — discontinued. Migrate to:
- **Reqnroll** — modern fork; drop-in replacement, actively maintained.

## Quick Reference

```gherkin
# Login.feature
Feature: User login
  As a registered user
  I want to log in
  So that I can place orders

  Scenario: successful login with valid credentials
    Given a registered user with email "alice@example.com" and password "correct horse"
    When  the user logs in with those credentials
    Then  the user sees the dashboard
    And   a session cookie is set
```

```csharp
// LoginStepDefinitions.cs
[Binding]
public sealed class LoginSteps(WebApplicationFactory<Program> factory)
{
    private HttpClient _client = factory.CreateClient();
    private HttpResponseMessage _response = null!;

    [Given("a registered user with email {string} and password {string}")]
    public void GivenRegisteredUser(string email, string password)
    {
        // seed test DB
    }

    [When("the user logs in with those credentials")]
    public async Task WhenUserLogsIn()
    {
        _response = await _client.PostAsJsonAsync("/login", new { email, password });
    }

    [Then("the user sees the dashboard")]
    public void ThenUserSeesDashboard()
    {
        Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
    }
}
```

## When BDD wins

- Acceptance tests written WITH product/QA — the .feature file becomes the spec
- Multiple scenarios sharing setup ("Background" + scenario outlines)
- Cross-functional teams aligning on terminology

## When BDD overhead doesn't pay off

- Solo devs / pure unit tests — Reqnroll plumbing is heavy
- Tests that aren't read by non-engineers — straight xUnit is shorter

## Common Pitfalls

- Steps that aren't reusable across scenarios → step explosion
- "Then I see the success message in the third div" → step coupled to UI; abstract via page objects
- Massive .feature files → split per workflow

## See also

- [../Unit](../Unit/) · [../EndToEnd](../EndToEnd/) · [../TestDrivenDevelopment](../TestDrivenDevelopment/)
