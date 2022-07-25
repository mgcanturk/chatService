using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.Models
{
    public class ClientModel
    {
        public Socket socket { get; set; }
        public bool isConnected { get; set; }
        public bool isMessageLimitExpired { get; set; }
        public DateTime lastRecivedTime { get; set; }
    }
}
