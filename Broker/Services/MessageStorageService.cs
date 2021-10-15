using Broker.Models;
using Broker.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services
{
    public  class MessageStorageService : IMessageStorageService
    {
        //private readonly ConcurrentQueue<Message> _messages;
        private readonly List<Message> _messages;
        private object locker;
        public MessageStorageService()
        {
            //_messages = new ConcurrentQueue<Message>();
                _messages = new List<Message>();
                locker = new object();
        }


        public void Add(Message message)
        {
            //_messages.Enqueue(message);
            lock (locker)
            {
                _messages.Add(message);
            }
        }

        public Message getNext()
        {
            //Message message;

            // _messages.TryDequeue(out message);
            lock (locker)
            {
                return _messages.Last();
            }
        }

        public bool isEmpty()
        {
            lock (locker)
            {
                if (_messages.Count == 0) return true;
                else return false;
            }
            //return _messages.IsEmpty;
        }
        public void Remove(Message message)
        {
            lock (locker)
            {
                _messages.Remove(message);
            }
        }
    }
}
