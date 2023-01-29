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
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _sessionFactory += sessionFactory;
            //소켓 생성(문지기역할)
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //바인드( 문지기 교육)
            _listenSocket.Bind(endPoint);

            //Listen(영업시작)
            //backlog : 최대대기수
            _listenSocket.Listen(10);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();// 한번만 만들어주면 계속 재사용가능함
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; //  초기화

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false)
            {// 등록하자말자 Accept가 이루어진 경우(바로 클라이언트가 접속요청이 온 경우)
                OnAcceptCompleted(null, args);
            }

        }

        // 스레드풀에서 하나 떙겨와서 실행됌. 레드존. 같은데이터를 쓰는경우에 위험함
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {//유저가 커넥트 요청이 들어오면 여기로 떨어짐
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke() ;
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args);//다음 클라이언트를 위해 다시 이벤트 등록

        }

    }
}
