using Broker.Services.Interfaces;
using Grpc.Core;
using GrpcAgent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Broker.Services
{
    public class SenderWorker : IHostedService
    {
        private Timer _timer;
        private const int TimeToWait = 2000;
        private readonly IMessageStorageService _messageStorage;
        private readonly IConnectionStorageService _connectionStorage;
        public SenderWorker(IServiceScopeFactory serviceScopeFactory)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                _messageStorage = scope.ServiceProvider.GetRequiredService<IMessageStorageService>();
                _connectionStorage = scope.ServiceProvider.GetRequiredService<IConnectionStorageService>();

            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(SendWork, null, 0, TimeToWait);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }


        private void SendWork(object state)
        {

            while (!_messageStorage.isEmpty())
            {
                var message = _messageStorage.getNext();
                if (message != null)
                {
                    var connections = _connectionStorage.GetConnectionsByTopic(message.Topic);
                    if (connections.Count > 0)
                    {
                        foreach (var connection in connections)
                        {
                            var client = new Notifier.NotifierClient(connection.grpcChannel);
                            var request = new NotifyRequest() { Dish = message.Dish };

                            try
                            {
                                var reply = client.Notify(request);
                                Console.WriteLine($"Notified subscriber {connection.Address} with {message.Dish}. Response: {reply.IsSuccess}");
                            }
                            catch (RpcException rpcEx)
                            {
                                if (rpcEx.StatusCode == StatusCode.Internal)
                                {
                                    _connectionStorage.Remove(connection.Address);
                                }
                                Console.WriteLine($"Rpc Error notyfying subscriber{connection.Address}. {rpcEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error notifying subscriber {connection.Address}. {ex.Message}");
                            }
                        }
                        _messageStorage.Remove(message);
                    }
                }
            }
        }
    }
}
