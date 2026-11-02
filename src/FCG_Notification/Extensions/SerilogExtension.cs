using System.Diagnostics.CodeAnalysis;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace FCG_Notification.Extensions;

[ExcludeFromCodeCoverage]
public static class SerilogConfiguration
{
    /// <summary>
    /// Em Development a saída é legível, para acompanhar durante o desenvolvimento.
    /// Em Production é JSON, onde cada propriedade do log vira um campo pesquisável.
    /// </summary>
    public static IHostBuilder UseSerilogLogging(this IHostBuilder host) =>
        host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                // "Now listening on: http://localhost:5022".
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "FCG_Notification")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

            if (context.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.Console();
            }
            else
            {
                loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
            }
        });

    /// <summary>
    /// Grava uma linha por requisição HTTP com método, rota, status e duração.
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app) =>
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = GetRequestLogLevel;
        });

    private static LogEventLevel GetRequestLogLevel(
        HttpContext httpContext,
        double elapsed,
        Exception? exception) =>
        exception is not null || httpContext.Response.StatusCode >= 500
            ? LogEventLevel.Error
            : httpContext.Request.Path.StartsWithSegments("/health")
                ? LogEventLevel.Debug
                : LogEventLevel.Information;
}
