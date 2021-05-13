using System;
using System.Net;
using System.Net.Sockets;

namespace Chain
{
    class Program
    {
        private static Socket _sender;
        private static Socket _listener;
        private static int _x;
        private static bool _isFirst = false;

        static void Main(string[] args)
        {
            int listeningPort = Int32.Parse(args[0]);
            string address = args[1];
            int port = Int32.Parse(args[2]);
            CreateConnect(listeningPort, address, port);
            _isFirst = args.Length == 4 && args[3] == "true";
           
            _x = Convert.ToInt32(Console.ReadLine());

            Start();

            Console.ReadLine();
        }

        private static void CreateConnect(int listeningPort, string address, int port)
        {
            _listener = GetSetupListner(listeningPort);
            _sender = GetSetupSender(address, port);
        }

        private static Socket GetSetupListner(int listeningPort)
        {
            IPAddress listenIpAddress = IPAddress.Any;
            Socket listener = new Socket(
                 listenIpAddress.AddressFamily,
                 SocketType.Stream,
                 ProtocolType.Tcp);

            IPEndPoint localEP = new IPEndPoint(listenIpAddress, listeningPort);
            listener.Bind(localEP);
            listener.Listen(10);

            return listener;
        }

        private static Socket GetSetupSender(string address, int port)
        {
            IPAddress ipAddress;
            if (address == "localhost")
            {
                ipAddress = IPAddress.Loopback;

            }
            else
            {
                ipAddress = IPAddress.Parse(address);
            }

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            Socket sender = new Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);


            try
            {
                sender.Connect(remoteEP);
            }
            catch (SocketException)
            {
                Console.WriteLine("Failed to connecting");
            }

            return sender;
        }

        private static void Start()
        {

            if (_isFirst)
            {
                Initiator();
            }
            else
            {
                NormalProcess();
            }
        }

        private static void Initiator()
        {
            Socket handler = _listener.Accept();
            
            var bytes = BitConverter.GetBytes(_x);
            _sender.Send(bytes);

            byte[] buf = new byte[sizeof(int)];
            handler.Receive(buf);
            int y = BitConverter.ToInt32(buf);
            _x = y;
            Console.WriteLine(_x);

            bytes = BitConverter.GetBytes(Math.Max(_x, y));
            _sender.Send(bytes);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void NormalProcess()
        {
            Socket handler = _listener.Accept();

            byte[] buf = new byte[sizeof(int)];
            handler.Receive(buf);
            int y = BitConverter.ToInt32(buf);
            var bytes = BitConverter.GetBytes(Math.Max(_x, y));
            _sender.Send(bytes);

            buf = new byte[sizeof(int)];
            handler.Receive(buf);
            int receivedNumber = BitConverter.ToInt32(buf);
            Console.WriteLine(receivedNumber);
            _sender.Send(buf);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}
