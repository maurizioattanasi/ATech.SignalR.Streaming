using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ATech.SignalR.Streaming.Grpc.Hubs
{
    public class StreamHub : Hub
    {       
        public ChannelReader<int> Counter(int count, int delay, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();

            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(ChannelWriter<int> writer, int count, int delay, CancellationToken cancellationToken)
        {
            Exception localException = null;

            try
            {
                for (var i = 0; i < count; i++)
                {
                    await writer.WriteAsync(i, cancellationToken);

                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }
            finally
            {
                writer.TryComplete(localException);
            }
        }
    }
}
