using Broker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broker.Services.Interfaces
{
    public interface IMessageStorageService
    {
        public void Add(Message message);

        Message getNext();

        void Remove(Message message);
        bool isEmpty();



    }
}
