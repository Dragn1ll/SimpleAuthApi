using System.Threading.Channels;

namespace Application.Queues;

public sealed class FileLogQueue
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

    public ValueTask EnqueueAsync(string line, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(line, ct);

    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);
}