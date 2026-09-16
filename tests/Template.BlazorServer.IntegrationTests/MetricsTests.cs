namespace Template.BlazorServer;

using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

using Template.BlazorServer.Host.Application.Telemetry;

public sealed partial class MetricsTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    private long count;

    public MetricsTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryTokenRegex();

    [Fact]
    public async Task ApiRequestIsCounted()
    {
        // Arrange
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, l) =>
        {
            if ((instrument.Meter.Name == Source.Name) && (instrument.Name == "api.request.execution"))
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((_, measurement, _, _) => Interlocked.Add(ref count, measurement));
        listener.Start();

        // Cookie認証: IdentityのログインフォームをSSRのフォームとして送る(_handlerはFormName)
        var client = factory.CreateClient();
        var loginPage = await client.GetStringAsync(new Uri("/Account/Login", UriKind.Relative), TestContext.Current.CancellationToken);
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["_handler"] = "login",
            ["Input.Name"] = "admin",
            ["Input.Password"] = "admin",
            ["__RequestVerificationToken"] = AntiforgeryTokenRegex().Match(loginPage).Groups[1].Value
        });
        await client.PostAsync(new Uri("/Account/Login", UriKind.Relative), form, TestContext.Current.CancellationToken);

        // Act
        var response = await client.GetAsync(new Uri("/api/data", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(Interlocked.Read(ref count) >= 1);
    }
}
