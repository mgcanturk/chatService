using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Services.Abstract
{
    public interface IclientService
    {
        string clientNickName { get; }
        bool connectServer();
        void shutdownClient();
        string receiveMessage();
        void beginReceive();
        void sendMessageToServer(string message);
    }
}
