using server.Services.Abstract;
using server.Services.Concreate;

namespace chatServiceTest
{
    public class tcpServerTest
    {
        ItcpServerService server;
        [SetUp]
        public void Setup()
        {
            //Arrange
            server = new tcpServerService();
        }
        [Test]
        public void start_Server_Test()
        {
            //Act
            var result = server.startServer();
            //Assert
            Assert.IsTrue(result);
        }
    }
}