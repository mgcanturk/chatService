using server.Models;
using server.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.Services.Concreate
{
    public class tcpServerService : ItcpServerService
    {
        private Socket socket { get; set; }
        private int port { get; set; }
        private byte[] buffer { get; set; }
        private int bufferSize { get; set; }
        private List<ClientModel> clientList = new();//The Client Info structure holds. the required information about every. client connected to the server
        public tcpServerService()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Create a TCP/IP socket.  
            port = 1920;//port number
            bufferSize = 2048;//Size of receive buffer.  
            buffer = new byte[bufferSize];//Receive buffer.  
        }
        public bool startServer()
        {
            try
            {
                IPEndPoint ipEndPoint = new(IPAddress.Any, port);//Assign the any IP of the machine and listen on port number 1920
                socket.Bind(ipEndPoint);//Bind and listen on the given address
                socket.Listen(0);//Bind and listen on the given address
                socket.BeginAccept(beginAcceptCallback, null);//Accept the incoming clients
                Console.WriteLine("Server start complete.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
                return false;
            }
        }
        public void beginAcceptCallback(IAsyncResult ar)
        {
            Socket _socket;
            try
            {
                _socket = socket.EndAccept(ar);// Get the socket that handles the client request.  
                clientList.Add(new ClientModel() { socket = _socket, isConnected = true });// The clients are accepted and add to clients list. 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                return;
            }
            _socket.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, beginReceiveCallback, _socket);
            socket.BeginAccept(beginAcceptCallback, null);
            Console.WriteLine("Client connected, waiting for request...");
        }
        public void beginReceiveCallback(IAsyncResult ar)
        {
            Socket current = (Socket)ar.AsyncState;
            int received = 0;
            try
            {
                // Read data from the client socket.   
                if (!current.Connected) return;
                received = current.EndReceive(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                current.Close();
                return;
            }
            getAllMessage(received);

            if (!checkSpamMessage(current)) return;

            sendMessage(current, "This message successfuly delivered.");

            current.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, beginReceiveCallback, current);
        }
        private async static void sendMessage(Socket socket, string message)
        {
            // Convert the string data to byte data using ASCII encoding.  
            var response = Encoding.ASCII.GetBytes(message);
            // Begin sending the data to the remote device.  
            await socket.SendAsync(response, SocketFlags.None);
        }
        public void getAllMessage(int received)
        {
            byte[] receiveBuffer = new byte[received];
            Array.Copy(buffer, receiveBuffer, received);
            string message = Encoding.ASCII.GetString(receiveBuffer);
            Console.WriteLine(message);
        }

        private bool checkSpamMessage(Socket handler)
        {
            var clientIndex = clientList.FindIndex(x => x.socket == handler);
            var timeDifferent = (DateTime.Now - clientList[clientIndex].lastRecivedTime).TotalSeconds;
            clientList[clientIndex].lastRecivedTime = DateTime.Now;
            if (timeDifferent <= 1)
            {
                if (clientList[clientIndex].isMessageLimitExpired)
                {
                    sendMessage(handler, "[Server]: You are disconnected from server.");
                    clientList[clientIndex].isConnected = false;
                    shutdownSocket(handler);
                    return false;
                }
                clientList[clientIndex].isMessageLimitExpired = true;
                sendMessage(handler, "[Warning]: Please do not send more than one message in a second. Next time you will disconnected from the server.");
            }
            return true;
        }
        private void shutdownSocket(Socket current)
        {
            //Shutdown the connection
            current.Shutdown(SocketShutdown.Both);
            current.Close();
        }
    }
}
