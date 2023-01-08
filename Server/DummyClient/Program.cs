using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //DNS (Domain Name System
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            while (true)
            {
                //
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //
                try
                {
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                    //보낸다
                    for (int i = 0; i < 5; i++)
                    {

                        byte[] sendBuff = Encoding.UTF8.GetBytes($"hello World! {i}");
                        int sendByte = socket.Send(sendBuff);
                    }
                    //받는다
                    byte[] recvBuff = new byte[1024];
                    int recvByte = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff);
                    Console.WriteLine($"[From Server] {recvData}");

                    // 
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message); ;
                }
                Thread.Sleep(100);
            }
        }
    }
}