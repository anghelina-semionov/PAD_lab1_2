using Grpc.Net.Client;
using GrpcAgent;
using Library;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var host = WebHost.CreateDefaultBuilder(args)
                  .UseStartup<Startup>()
                  .UseUrls(EndpointsConstants.SubscribersAddress)
                  .Build();

            host.Start();

            Subscribe(host);

            Console.WriteLine("Press escape to exit");
            Console.ReadLine();
        }


        private static void Subscribe(IWebHost host)
        {
            var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAdress);
            var client = new Subscriber.SubscriberClient(channel);

            var adress = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            Console.WriteLine($"Subscriber listening at:  { adress}");
            Console.Write("Enter the topic:");
            var topic = Console.ReadLine().ToLower();

         

            var request = new SubscribeRequest() { Topic = topic , Address = adress };

            try
            {
                var reply = client.Subscribe(request);
                Console.WriteLine($"Subsrcibe reply:  { reply.IsSuccess}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error subscribing: {ex.Message }");

            }



        }

       
    }
}
