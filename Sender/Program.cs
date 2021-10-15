using Grpc.Net.Client;
using GrpcAgent;
using Library;
using System;
using System.Threading.Tasks;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Publisher");
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAdress);
            var client = new Publisher.PublisherClient(channel);

            while(true)
            {
                Console.Write("Enter the topic:");
                var topic = Console.ReadLine().ToLower();

                Console.Write("Enter the message:");
                var dish = Console.ReadLine();


                var request = new PublishRequest() { Topic = topic, Dish = dish };

                try
                {
                    var reply = await client.PublishMessageAsync(request);
                    Console.WriteLine($"Publish reply: {reply.IsSuccess}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error publishing the message: { ex.Message }");
                }

            }



        }
    }
}
