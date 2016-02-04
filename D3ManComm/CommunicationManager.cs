using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace D3ManComm
{
    public class CommunicationManager
    {
        public bool ServerRunning
        {
            get; private set;
        } = false;


        public Func<string, string> ProcessIncomingMessage
        { get; set; }

        public Action OnListenerStarted
        { get; set; } = null;
    
        public bool IsMaster { get; private set; } = false;

        const int BufferSize = 4096;
        public const int MasterPort = 13;
        public const int AgentPort = 14;
        CancellationTokenSource _cts;
        string _machineName = "";

        public CommunicationManager(CancellationTokenSource cts, bool isMaster)
        {
            _cts = cts;
            _machineName = "localhost";
            IsMaster = isMaster;

        }
        public async Task Start(CancellationToken ct)
        {
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener tcpListener = new TcpListener(ipAddress, IsMaster ? MasterPort : AgentPort);
            tcpListener.Start();

            if (OnListenerStarted != null)
            {
                OnListenerStarted();
            }
            ServerRunning = true;
            Util.Log("Listner started: " + ipAddress.ToString());

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    ListenForClients(tcpListener);
                    await Task.Delay(100);
                }

            }
            catch (Exception ex)
            {
                Util.Log(ex.Message);
                Util.Log(ex.StackTrace);


            }
            finally
            {
                tcpListener.Stop();
                Util.Log("Server stops");
                ServerRunning = false;
            }

        }

        public void Stop()
        {
            _cts.Cancel();
           
        }
        private async void ListenForClients(TcpListener tcpListener)

        {

            try
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();

                Util.Log("Connected");

                await ProcessClient(tcpClient);
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Cannot access a disposed object."))
                {
                    Util.Log(ex.Message);
                    Util.Log(ex.StackTrace);
                }
            }





        }



        private async Task ProcessClient(TcpClient tcpClient)

        {


            var stream = tcpClient.GetStream();

            var buffer = new byte[BufferSize];

            var amountRead = await stream.ReadAsync(buffer, 0, BufferSize);

            var message = Encoding.ASCII.GetString(buffer, 0, amountRead);

            string response = "OK";

            //Util.Log(string.Format("Message received from {0} -- {1}", ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString(), message));

            if (ProcessIncomingMessage != null)
            {
                response = ProcessIncomingMessage(message);
            }

            byte[] bytesSent = Encoding.ASCII.GetBytes(response);
            stream.Write(bytesSent, 0, bytesSent.Length);


        }

        private async Task<string> SendMessage(string url, string message, int port)
        {
            TcpClient client = new TcpClient(url, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            var buffer = new byte[BufferSize];

            var amountRead = await stream.ReadAsync(buffer, 0, BufferSize);

            var response = Encoding.ASCII.GetString(buffer, 0, amountRead);

            stream.Close();
            client.Close();

            return response;

        }

        

        public async Task<bool> ConnectToServer(string server)
        {
           
            string response = await SendMessage(server, Command.CMD_Connect + Command.DELIMITER + _machineName, MasterPort);


            return (response == Command.MSG_OK);

        }

        public async Task<bool> SetLeader(string agent)
        {
            string response = await SendMessage(agent, Command.CMD_SetLeader + Command.DELIMITER + _machineName, AgentPort);


            return (response == Command.MSG_OK);
        }
    }
}
