using client.Services.Abstract;
using client.Services.Concreate;
using server.Services.Abstract;
using server.Services.Concreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatServiceTest
{
    public class clientTest
    {
        ItcpServerService server;
        IclientService client1, client2;
        [SetUp]
        public void Setup()
        {
            //Arrange
            client1 = new clientService();
            client2 = new clientService();
            server = new tcpServerService();
        }
        [Test]
        public void Two_Client_Connect_Server_Test()
        {
            //Arrange
            var resultServer = server.startServer();

            //Act
            var resultClient1 = client1.connectServer();
            var resultClient2 = client2.connectServer();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(resultServer);
                Assert.IsTrue(resultClient1);
                Assert.IsTrue(resultClient2);
            });
        }
        [Test]
        public void Two_Client_Send_Message_Test()
        {
            //Arrange
            var resultServer = server.startServer();

            //Act
            var resultClient1 = client1.connectServer();
            client1.sendMessageToServer("Message_Test_Client_1");
            var message = client1.receiveMessage();

            var resultClient2 = client2.connectServer();
            client2.sendMessageToServer("Message_Test_Client_2");
            var message2 = client2.receiveMessage();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(resultClient1);
                Assert.That(message, Does.Contain("This message successfuly delivered."));

                Assert.IsTrue(resultClient2);
                Assert.That(message2, Does.Contain("This message successfuly delivered."));
            });
        }
        [Test]
        public void Spam_Message_From_Warning_Test()
        {
            //Arrange
            var resultServer = server.startServer();

            //Act
            var resultClient = client1.connectServer();
            client1.sendMessageToServer("Test_Client_Message_1");
            var message = client1.receiveMessage();
            client1.sendMessageToServer("Test_Client_Message_2");
            var message2 = client1.receiveMessage();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(resultClient);
                Assert.That(message2, Does.Contain("[Warning]: Please do not send more than one message in a second. Next time you will disconnected from the server."));
            });
        }
        [Test]
        public void Spam_Message_From_Disconnect_Server_Test()
        {
            //Arrange
            var resultServer = server.startServer();

            //Act
            var result = client1.connectServer();
            client1.sendMessageToServer("Test_Client_Message_1");
            var message = client1.receiveMessage();
            client1.sendMessageToServer("Test_Client_Message_2");
            var message1 = client1.receiveMessage();
            client1.sendMessageToServer("Test_Client_Message_3");
            var message2 = client1.receiveMessage();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(result);
                Assert.That(message1, Does.Contain("[Warning]: Please do not send more than one message in a second. Next time you will disconnected from the server."));
                Assert.That(message2, Does.Contain("[Server]: You are disconnected from server."));
            });
        }
    }
}
