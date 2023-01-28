using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerCore
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

            //소켓 생성(문지기역할)
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //바인드( 문지기 교육)
                listenSocket.Bind(endPoint);

                //Listen(영업시작)
                //backlog : 최대대기수
                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening...");

                    //손님을 입장시킨다
                    Socket clientSocket = listenSocket.Accept(); // 이 소켓(클라쪽)으로 통신을 함

                    //받는 부분
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = clientSocket.Receive(recvBuff);// recvBuff 에 저장
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, 1024);
                    Console.WriteLine($"[From Client] {recvData}");

                    //보낸다
                    byte[] sendBuff = Encoding.UTF8.GetBytes("welcome to server!");
                    clientSocket.Send(sendBuff);

                    //소켓연결 끊기
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
