using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ATech.SignalR.Streaming.Grpc.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ATech.SignalR.Streaming.Grpc.Hubs
{
    public class ATechProcessHub : Hub
    {
        private readonly RandomUserService randomUserService;
        private readonly ILogger<ATechProcessHub> logger;

        public ATechProcessHub(RandomUserService randomUserService,
                               ILogger<ATechProcessHub> logger)
        {
            this.randomUserService = randomUserService;
            this.logger = logger;
        }

        #region GetUsers

        public ChannelReader<User> GetUsers(int count, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateBounded<User>(10);

            WriteUsers(channel.Writer, count, cancellationToken);

            return channel.Reader;
        }


        private async Task WriteUsers(ChannelWriter<User> channelWriter, int count, CancellationToken cancellationToken)
        {
            Exception localException = null;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var user = await randomUserService.GetUser().ConfigureAwait(false);
                    
                    //logger.LogInformation("New user: {user}", user);

                    await channelWriter.WriteAsync(user, cancellationToken);

                    //await Task.Delay(50, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                var content = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
                logger.LogError(content);
                localException = ex;               
            }
            finally
            {
                channelWriter.TryComplete(localException);
            }
        }
    }

    #endregion
}

