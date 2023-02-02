using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using static System.Collections.Specialized.BitVector32;
using ServerCore;
using System.Collections.Generic;
using Server;

namespace Server
{

    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250); //0.25초 후에 다시 호출
        }
        static void Main(string[] args)
        {

            //DNS(Domain Name System)
            // www.yonggil.com 에 해당하는 IP를 찾아주는시스템
            string host = Dns.GetHostName();// 내로컬컴퓨터의 호스트
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];// 리스트로 나오는 이유는 ip분산을위함(구글같은경우에는 여러개가있음)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);// 최종주소(식당으로 치면 ipaddr : 식당주소, 포트번호 : 뒷문,정문)
            
            _listener.Init(endPoint, () => {return SessionManager.Instance.Generate();});
            Console.WriteLine("Listening...");

            //FlushRoom();
            JobTimer.Instance.Push(FlushRoom);
            while (true)
            {
                JobTimer.Instance.Flush();
            }

        }
    }
}
