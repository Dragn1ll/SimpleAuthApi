using System.Text;
using Application.Queues;

namespace SimpleAuthApi.BackgroundServices;

public sealed class FileLogWriterHostedService : BackgroundService
{
    private readonly FileLogQueue _queue;
    private readonly string _filePath;

    public FileLogWriterHostedService(FileLogQueue queue, IConfiguration cfg)
    {
        _queue = queue;
        _filePath = cfg["FileLogging:Path"] ?? "logs/app.log";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? ".");

        await using var fs = new FileStream(
            _filePath,
            FileMode.Append,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 1 << 20,
            useAsync: true);

        await using var writer = new StreamWriter(fs, new UTF8Encoding(false), bufferSize: 1 << 20);

        var batch = new List<string>(capacity: 500);
        var lastFlush = DateTime.UtcNow;

        await foreach (var line in _queue.ReadAllAsync(stoppingToken))
        {
            batch.Add(line);

            if (batch.Count >= 500 || (DateTime.UtcNow - lastFlush).TotalSeconds >= 1)
            {
                foreach (var t in batch)
                    await writer.WriteLineAsync(t);

                batch.Clear();
                await writer.FlushAsync(stoppingToken);
                lastFlush = DateTime.UtcNow;
            }
        }
    }
}