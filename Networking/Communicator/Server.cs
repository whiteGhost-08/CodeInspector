﻿/////
/// Author:
/////

using System.Net.Sockets;
using System.Net;
using Networking.Utils;
using Networking.Models;

namespace Networking.Communicator
{
    public class Server : ICommunicator
    {
        private bool _stopThread = false;
        private Sender _sender;
        private Thread _listenThread;
        private Receiver _receiver;
        private TcpListener _serverListener;
        Dictionary<string, NetworkStream> _clientIDToStream = new();
        private Dictionary<string, IEventHandler> _moduleEventMap = new();
        private string _senderID;


        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void Send(string Data, string eventType, string destID)
        {
            Console.WriteLine("[Server] Send" + Data + " " + eventType + " " + destID);
            Message message = new Message(
    Data, eventType, destID, _senderID
);
            _sender.Send(message);
        }
        public void Send(string Data, string eventType, string destID, string senderID)
        {
            Console.WriteLine("[Server] Send" + Data + " " + eventType + " " + destID);
            Message message = new Message(
    Data, eventType, destID, senderID
);
            _sender.Send(message);
        }

        public string Start(string? destIP, int? destPort, string senderID)
        {
            Console.WriteLine("[Server] Start" + destIP + " " + destPort);
            _senderID = senderID;
            _sender = new(_clientIDToStream, false);
            _receiver = new(_clientIDToStream, _moduleEventMap);

            _serverListener = new TcpListener(IPAddress.Any, 12345);
            _serverListener.Start();
            IPEndPoint localEndPoint = (IPEndPoint)_serverListener.LocalEndpoint;
            Console.WriteLine("[Server] Server is listening on:");
            Console.WriteLine("[Server] IP Address: " + GetLocalIPAddress());
            Console.WriteLine("[Server] Port: " + localEndPoint.Port);
            _listenThread = new Thread(AcceptConnection);
            _listenThread.Start();
            this.Subscribe(new NetworkingEventHandler(), "networking");
            return localEndPoint.Address + ":" + localEndPoint.Port;
        }

        public void Stop()
        {
            Console.WriteLine("[Server] Stop");
            _sender.Stop();
            _receiver.Stop();
            Console.WriteLine("[Server] Stopped _sender and _receiver");
            _serverListener.Stop();
            _listenThread.Join();
            Console.WriteLine("[Server] Stopped");
        }

        public void Subscribe(IEventHandler eventHandler, string moduleName)
        {
            Console.WriteLine("[Server] Subscribe");
            _moduleEventMap.Add(moduleName, eventHandler);
        }

        void AcceptConnection()
        {
            string clientID = "A";

            while (!_stopThread)
            {
                Console.WriteLine("waiting for connection");
                TcpClient client = _serverListener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                lock (_clientIDToStream) { _clientIDToStream.Add(clientID, stream); }
                clientID += 'A';
                Console.WriteLine("client connected");
            }
        }
    }
}
