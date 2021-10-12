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
    public class User
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

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

        public async Task<ChannelReader<User>> GetUsers(int count, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateBounded<User>(5);

            await WriteUsers(channel.Writer, count, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteUsers(ChannelWriter<User> channelWriter, int count, CancellationToken cancellationToken)
        {
            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var randomUser = await randomUserService.GetUser();
                var result = randomUser.Results.FirstOrDefault();

                var user = new User
                {
                    FirstName = result.Name.First,
                    LastName = result.Name.Last,
                    Title = result.Name.Title
                };

                await channelWriter.WriteAsync(user, cancellationToken);
            }
        }

        #endregion
    }
}
