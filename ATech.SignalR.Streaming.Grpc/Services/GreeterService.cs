using System.Linq;
using System.Threading.Tasks;
using ATech.SignalR.Streaming.Grpc.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace ATech.SignalR.Streaming.Grpc
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly RandomUserService randomUserService;
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(RandomUserService randomUserService, ILogger<GreeterService> logger)
        {
            this.randomUserService = randomUserService;
            _logger = logger;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var randomUser = await randomUserService.GetUser();
            var user = randomUser;

            return new HelloReply
            {
                Message = "Hello " + request.Name
            };
        }
    }
}
