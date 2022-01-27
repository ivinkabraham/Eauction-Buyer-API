using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eauction_Buyer_API.MessageSharer
{
    public interface IRabbitMQCreator
    {
        void Publish(string message);
    }
}
