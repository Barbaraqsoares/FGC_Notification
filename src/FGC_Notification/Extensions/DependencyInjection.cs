using FGC_Notification.Consumers;
using FGC_Notification.Services;
using FGC_Notification.Services.Interfaces;
using MassTransit;

namespace FGC_Notification.Extensions;

/// <summary>
/// Registra os serviços da camada de API (controllers, autenticação e Swagger).
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra serviços de infraestrutura (DbContext, repositories, etc.).
    /// Mantém Program.cs enxuto delegando a configuração aqui.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }

    /// <summary>
    /// Escreve a resposta de erro no mesmo formato ProblemDetails (RFC 7807) usado
    /// pelo ExceptionHandlingMiddleware, para que quem consome a API trate todos os
    /// erros da mesma maneira — inclusive os gerados pelo pipeline de autorização.
    /// </summary>
    private static async Task WriteProblemDetails(HttpContext context, int statusCode, string title, string detail)
    {
        // Se a resposta já começou a ser enviada, não é mais possível alterá-la.
        if (context.Response.HasStarted)
        {
            return;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path.HasValue ? context.Request.Path.Value : context.Request.Path.ToString()
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json");
    }

    /// <summary>
    /// Documentação da API no Swagger, com suporte a autenticação por token.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        // Registra ApiExplorer. Tenta registrar o Swashbuckle AddSwaggerGen, mas captura exceções
        // para evitar que conflitos de assemblies quebrem a inicialização da aplicação.
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(static swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FGC_Notification",
                Version = "v1",
                Description = "API responsável por receber notificações",
                Contact = new OpenApiContact
                {
                    Name = "Equipe Desafio",
                    Email = "contato@desafio.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swagger.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection ConfigMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<UserCreatedConsumer>();
            x.AddConsumer<PaymentProcessedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                // Permite configurar o host via configuração se disponível, com fallback para localhost
                var host = configuration["RabbitMq:Host"] ?? "localhost";
                var virtualHost = configuration["RabbitMq:VirtualHost"] ?? "/";
                var username = configuration["RabbitMq:Username"] ?? "guest";
                var password = configuration["RabbitMq:Password"] ?? "guest";

                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(username); //guest
                    h.Password(password); //guest
                });
            });
        });

        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
}