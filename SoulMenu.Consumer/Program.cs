using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoulMenu.Consumer.Consumer;
using SoulMenu.Consumer.Domain.Gateways;
using SoulMenu.Consumer.Domain.UseCases;
using SoulMenu.Consumer.Infrastructure.PostgreDb;
using SoulMenu.Consumer.Infrastructure.PostgreDb.Gateways;
using SoulMenu.Consumer.Infrastructure.RabbitMq;

Console.WriteLine("Aguardando 2 minutos para iniciar a execução da Saga...");
await Task.Delay(120000);

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configuração do logger
        services.AddLogging(config =>
        {
            config.AddConsole();  // Adiciona o log no console
            config.AddDebug();    // Adiciona o log no debug (útil para debugging local)
        });

        // Registro da classe PaymentWorker como um serviço em segundo plano
        services.AddHostedService<SimulateOrderWorker>();
        services.AddScoped<IQueueService, QueueService>();
        services.AddScoped<IItemMenuUseCase, ItemMenuUseCase>();
        services.AddScoped<IItemMenuGateway, ItemMenuGateway>();


        // Configuração do PostgreSQL
        var postgreDbSettings = hostContext.Configuration.GetSection("PostgreDbSettings").Get<PostgreDbSettings>();

        services.AddSingleton<Context>(sp =>
            new Context(postgreDbSettings.PostgresConnection));

        services.AddSingleton<IRabbitMqSettings>(_ => hostContext.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>());
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();  // Log no console para monitorar o serviço em execução
    });

var host = builder.Build();

await host.RunAsync();
