using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ATech.SignalR.Streaming.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://localhost:5001/streamHub";

            var connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (ex) =>
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("restart");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                await connection.StartAsync(cancellationTokenSource.Token);

                var channel = await connection.StreamAsChannelAsync<int>("Counter", 100, 1000, cancellationTokenSource.Token);
                while (await channel.WaitToReadAsync(cancellationTokenSource.Token))
                {
                    while (channel.TryRead(out int data))
                        Console.WriteLine($"Received: {data}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Hello, World!");
        }
    }
}
