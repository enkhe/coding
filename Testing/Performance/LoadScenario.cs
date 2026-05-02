using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace Testing.Performance;

public static class LoadScenario
{
    public static void Run()
    {
        using var http = new HttpClient { BaseAddress = new Uri("https://localhost:5001") };

        var scenario = Scenario.Create("get_orders", async ctx =>
        {
            var request = Http.CreateRequest("GET", "/orders")
                .WithHeader("Accept", "application/json");

            return await Http.Send(http, request);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            // Ramp from 0 to 50 RPS over 30s, hold 50 RPS for 2 min.
            Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
            Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(2))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder("nbomber-reports")
            .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
            .Run();
    }
}
