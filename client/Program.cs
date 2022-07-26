using client.Services.Abstract;
using client.Services.Concreate;

namespace client
{
    class Program
    {
        public static IclientService _clientService { get; set; }
        static void Main(string[] args)
        {
            var client = new clientService();
            _clientService = client;
            _clientService.connectServer();
            Console.Title = $"Client #{_clientService.clientNickName}";
            _clientService.beginReceive();
        }
    }
}