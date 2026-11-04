using System.Diagnostics.CodeAnalysis;
using FCG_Notification.Extensions;
using Serilog;

return Program.RunApplication(args);

[ExcludeFromCodeCoverage]
internal partial class Program
{
    internal static int RunApplication(string[] args) => Execute(() => StartApplication(args));

    internal static int Execute(Action startApplication)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            startApplication();
        }
        catch (Exception exception) when (exception is not HostAbortedException)
        {
            LogStartupException(exception);
        }
        finally
        {
            Log.CloseAndFlush();
        }

        return 0;
    }

    private static void StartApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilogLogging();

        builder.Services.ConfigMassTransit(builder.Configuration);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddControllers();

        var app = builder.Build();

        app.Run();
    }

    private static void LogStartupException(Exception exception)
    {
        Console.Error.WriteLine("Startup exception (full):");
        Console.Error.WriteLine(exception.ToString());

        if (exception is AggregateException aggregate)
        {
            var flat = aggregate.Flatten();
            Console.Error.WriteLine("AggregateException flattened. Inner exceptions:");
            foreach (var inner in flat.InnerExceptions)
            {
                Console.Error.WriteLine("---- Inner exception ----");
                Console.Error.WriteLine(inner.ToString());
            }
        }
        else
        {
            var current = exception.InnerException;
            while (current != null)
            {
                Console.Error.WriteLine("---- Inner exception ----");
                Console.Error.WriteLine(current.ToString());
                current = current.InnerException;
            }
        }
    }
}