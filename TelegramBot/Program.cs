using TelegramBot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddHttpClient("api", client =>
        {
            var baseUrl = ctx.Configuration["Api:BaseUrl"] ?? "http://localhost:5104";
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHostedService<BotHostedService>();
    })
    .Build();

await host.RunAsync();