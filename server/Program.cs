using server.Services.Abstract;
using server.Services.Concreate;

namespace server
{
    class Program
    {
        public static ItcpServerService _tcpServerService { get; set; }
        static void Main(string[] args)
        {
            var server = new tcpServerService();
            _tcpServerService = server;
            Console.Title = "Server";
            _tcpServerService.startServer();
            Console.ReadLine();
        }
    }
}
