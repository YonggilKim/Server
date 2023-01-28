using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //DNS(Domain Name System)
            // www.yonggil.com 에 해당하는 IP를 찾아주는시스템
            string host = Dns.GetHostName();// 내로컬컴퓨터의 호스트
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];// 리스트로 나오는 이유는 ip분산을위함(구글같은경우에는 여러개가있음)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 최종주소(식당으로 치면 ipaddr : 식당주소, 포트번호 : 뒷문,정문)

            while (true)
            {
                // 소켓 설정( 휴대폰설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try 
                {
                    //문지기(서버쪽소켓)에게 입장문의
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected To{socket.RemoteEndPoint.ToString()}");//remoteEndPoint : 우리가연결한 반대쪽 대상

                    //보낸다
                    byte[] sendBuff = Encoding.UTF8.GetBytes("hello im client");
                    int sendBytes = socket.Send(sendBuff);

                    //받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(100);
            }

        }
    }
}
