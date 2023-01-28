using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;

        public void Init(IPEndPoint endPoint)
        {
            //소켓 생성(문지기역할)
            Socket _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //바인드( 문지기 교육)
            _listenSocket.Bind(endPoint);

            //Listen(영업시작)
            //backlog : 최대대기수
            _listenSocket.Listen(10);
        }

        public Socket Accept()
        {
            return _listenSocket.Accept();
        }
    }
}
