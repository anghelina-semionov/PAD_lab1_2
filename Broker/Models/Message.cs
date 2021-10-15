using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Models
{
    public class Message
    {
        
        public string Topic { get; }

        public string Dish { get; }

        public Message(String topic,String dish)
        {
            Topic = topic;
            Dish = dish;

        }
        
    }
}
