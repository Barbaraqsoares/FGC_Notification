using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace FGC_Notification.Tests;

public class ProgramTests
{
    [Fact]
    public void Execute_ReturnsZeroWhenStartupSucceeds()
    {
        var result = Program.Execute(() => { });

        Assert.Equal(0, result);
    }

    [Fact]
    public void Execute_LogsAggregateStartupExceptions()
    {
        var exception = new AggregateException(
            new InvalidOperationException("first failure"),
            new ArgumentException("second failure"));

        var result = Program.Execute(() => throw exception);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Execute_LogsNestedStartupExceptions()
    {
        var exception = new InvalidOperationException(
            "startup failure",
            new ArgumentException("inner failure"));

        var result = Program.Execute(() => throw exception);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Execute_RethrowsHostAbortedException()
    {
        Assert.Throws<HostAbortedException>(() =>
            Program.Execute(() => throw new HostAbortedException()));
    }

    [Fact]
    public async Task Program_StartsApplicationAndServesRequests()
    {
        await using var application = new TestApplicationFactory();
        using var client = application.CreateClient();

        using var response = await client.GetAsync("/");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class TestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services => services.RemoveAll<IHostedService>());
        }
    }
}
