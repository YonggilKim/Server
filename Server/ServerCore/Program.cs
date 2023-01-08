using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                Session session = new Session();
                session.Start(clientSocket);
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to RPG Server!");
                session.Send(sendBuff);

                Thread.Sleep(1000);
                session.Disconnect();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        static void Main(string[] args)
        {
            //DNS (Domain Name System
            // google.com -> 123.123.123.12
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            //


            try
            {
                _listener.init(endPoint, OnAcceptHandler);
                Console.WriteLine("Listening...");

                while (true)
                {
                    ;
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            //
            

        }

    }
}