using System.Text;
using Contracts.RabbitMq;
using MassTransit;

namespace ReportService;

public sealed class UserRegisteredConsumer : IConsumer<UserRegistered>
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private const string ReportPath = "reports/registrations.csv";

    public async Task Consume(ConsumeContext<UserRegistered> context)
    {
        var msg = context.Message;

        Directory.CreateDirectory("reports");

        var line = $"{msg.CreatedDateUtc:yyyy-MM-dd},{msg.UserId},{Escape(msg.Email)},{Escape(msg.Username)}";

        await Gate.WaitAsync(context.CancellationToken);
        try
        {
            var exists = File.Exists(ReportPath);
            await using var fs = new FileStream(ReportPath, FileMode.Append, FileAccess.Write, FileShare.Read, 
                1 << 20, useAsync: true);
            await using var sw = new StreamWriter(fs, new UTF8Encoding(false), bufferSize: 1 << 20);

            if (!exists)
                await sw.WriteLineAsync("date,userId,email,username");

            await sw.WriteLineAsync(line);
            await sw.FlushAsync();
        }
        finally
        {
            Gate.Release();
        }
    }

    private static string Escape(string s) => "\"" + s.Replace("\"", "\"\"") + "\"";
}
