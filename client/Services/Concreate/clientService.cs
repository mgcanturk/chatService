using client.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client.Services.Concreate
{
    public class clientService : IclientService
    {
        public string clientNickName { get; private set; }
        private Socket socket { get; set; }
        public clientService()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientNickName = "Client " + Guid.NewGuid().ToString();
        }
        public bool connectServer()
        {
            try
            {
                while (!socket.Connected)
                {
                    try
                    {
                        socket.Connect(IPAddress.Loopback, 1920);
                        Console.WriteLine("Connected. You can chat now.");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
            }

            return false;
        }

        public void shutdownClient()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Environment.Exit(0);
        }

        public string receiveMessage()
        {
            Thread.Sleep(10);//wait server message
            try
            {
                byte[] receiveBuffer = new byte[2048];
                socket.Receive(receiveBuffer);
                string message = Encoding.ASCII.GetString(receiveBuffer);
                return message;
            }
            catch (Exception ex)
            {
                return "Error:" + ex.ToString();
            }
        }

        public void beginReceive()
        {
            while (socket.Connected)
            {
                Console.Write("You Message: ");
                string message = Console.ReadLine();
                string sendMessage = clientNickName + ": " + message;
                sendMessageToServer(sendMessage);
                Console.WriteLine(receiveMessage());
            }
        }

        public void sendMessageToServer(string message)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(message);// Convert the string data to byte data using ASCII encoding. 
            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(sendAsyncCallback), socket);// Begin sending the data to the remote device.  
        }
        private void sendAsyncCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;// Retrieve the socket from the state object. 
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
