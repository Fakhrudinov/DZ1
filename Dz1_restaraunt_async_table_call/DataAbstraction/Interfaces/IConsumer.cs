﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAbstraction.Interfaces
{
    public interface IConsumer
    {
        public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback);
    }
}
