using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using static System.Collections.Specialized.BitVector32;
using ServerCore;

namespace Server
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnConnected : {endpoint}");
            byte[] sendBuff = Encoding.UTF8.GetBytes("welcome to server!");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnDisconnected : {endpoint}");

        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");

        }
    }
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            //DNS(Domain Name System)
            // www.yonggil.com 에 해당하는 IP를 찾아주는시스템
            string host = Dns.GetHostName();// 내로컬컴퓨터의 호스트
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];// 리스트로 나오는 이유는 ip분산을위함(구글같은경우에는 여러개가있음)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 최종주소(식당으로 치면 ipaddr : 식당주소, 포트번호 : 뒷문,정문)

            _listener.Init(endPoint, () => {
                return new GameSession();
            });
            Console.WriteLine("Listening...");

            while (true)
            {

            }

        }
    }
}
