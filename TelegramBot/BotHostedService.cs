using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot;

public sealed class BotHostedService : IHostedService
{
    private readonly ILogger<BotHostedService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    private TelegramBotClient? _bot;
    private CancellationTokenSource? _cts;

    public BotHostedService(
        ILogger<BotHostedService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = _configuration["Telegram:Token"];
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Telegram:Token is not set");

        _bot = new TelegramBotClient(token);
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };

        _bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: _cts.Token);

        var me = await _bot.GetMe(_cts.Token);
        _logger.LogInformation("Bot started: @{Username}", me.Username);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Type != UpdateType.Message)
            return;

        var msg = update.Message;
        if (msg?.Text is null)
            return;

        var chatId = msg.Chat.Id;
        var text = msg.Text.Trim();

        if (text.Equals("/start", StringComparison.OrdinalIgnoreCase) ||
            text.Equals("/help", StringComparison.OrdinalIgnoreCase))
        {
            await botClient.SendMessage(
                chatId,
                "Команды:\n" +
                "/minDate — минимальная дата регистрации\n" +
                "/maxDate — максимальная дата регистрации\n" +
                "/count — количество пользователей\n" +
                "/page <pageNumber> <pageSize> — страница пользователей\n" +
                "/range <startInclusive> <endExclusive> — диапазон по индексам\n",
                cancellationToken: ct);
            return;
        }

        var api = CreateApiClient();

        if (text.Equals("/minDate", StringComparison.OrdinalIgnoreCase))
        {
            var result = await GetAsString(api, "/api/User/stats/registration-date/min", ct);
            await botClient.SendMessage(chatId, result, cancellationToken: ct);
            return;
        }

        if (text.Equals("/maxDate", StringComparison.OrdinalIgnoreCase))
        {
            var result = await GetAsString(api, "/api/User/stats/registration-date/max", ct);
            await botClient.SendMessage(chatId, result, cancellationToken: ct);
            return;
        }

        if (text.Equals("/count", StringComparison.OrdinalIgnoreCase))
        {
            var result = await GetAsString(api, "/api/User/stats/count", ct);
            await botClient.SendMessage(chatId, result, cancellationToken: ct);
            return;
        }

        if (text.StartsWith("/page", StringComparison.OrdinalIgnoreCase))
        {
            var args = SplitArgs(text);
            if (args.Length != 3 ||
                !int.TryParse(args[1], out var pageNumber) ||
                !int.TryParse(args[2], out var pageSize))
            {
                await botClient.SendMessage(chatId, "Использование: /page 1 20", cancellationToken: ct);
                return;
            }

            var url = $"/api/User/page?pageNumber={pageNumber}&pageSize={pageSize}";
            var result = await GetAsString(api, url, ct);
            await botClient.SendMessage(chatId, result, cancellationToken: ct);
            return;
        }

        if (text.StartsWith("/range", StringComparison.OrdinalIgnoreCase))
        {
            // /range 0 50
            var args = SplitArgs(text);
            if (args.Length != 3 ||
                !int.TryParse(args[1], out var startInclusive) ||
                !int.TryParse(args[2], out var endExclusive))
            {
                await botClient.SendMessage(chatId, "Использование: /range 0 50", cancellationToken: ct);
                return;
            }

            var url = $"/api/User/range?startInclusive={startInclusive}&endExclusive={endExclusive}";
            var result = await GetAsString(api, url, ct);
            await botClient.SendMessage(chatId, result, cancellationToken: ct);
            return;
        }

        await botClient.SendMessage(chatId, "Неизвестная команда. Напиши /help", cancellationToken: ct);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Telegram polling error");
        return Task.CompletedTask;
    }

    private HttpClient CreateApiClient()
    {
        var client = _httpClientFactory.CreateClient();

        var baseUrl = _configuration["Api:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Api:BaseUrl is not set");

        client.BaseAddress = new Uri(baseUrl);
        return client;
    }

    private static async Task<string> GetAsString(HttpClient api, string relativeUrl, CancellationToken ct)
    {
        try
        {
            var resp = await api.GetAsync(relativeUrl, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                return $"HTTP {(int)resp.StatusCode}: {body}";

            if (string.IsNullOrWhiteSpace(body))
                return "OK";

            return body.Length <= 3500 ? body : body[..3500] + "\n... (truncated)";
        }
        catch (Exception ex)
        {
            return $"Request failed: {ex.Message}";
        }
    }

    private static string[] SplitArgs(string input)
        => input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
