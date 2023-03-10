using ServerCore;
using System.Collections.Generic;
using System;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager() 
    {
        Register();
    }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
		_makeFunc.Add((ushort)PacketID.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
		_handler.Add((ushort)PacketID.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
		_handler.Add((ushort)PacketID.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler);
		_makeFunc.Add((ushort)PacketID.S_PlayerList, MakePacket<S_PlayerList>);
		_handler.Add((ushort)PacketID.S_PlayerList, PacketHandler.S_PlayerListHandler);
		_makeFunc.Add((ushort)PacketID.S_BroadcastMove, MakePacket<S_BroadcastMove>);
		_handler.Add((ushort)PacketID.S_BroadcastMove, PacketHandler.S_BroadcastMoveHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallBack = null) 
    {
        //2. 사이즈와 아이디를 가지고와서 switch-case문에서 하던걸 -> 딕셔너리로 가져옴
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
            
        //3. 실제로 핸들러를 등록해 놓으면 그곳에 인보크를 할 예정
        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        { 
            IPacket packet = func.Invoke(session, buffer);
            if (onRecvCallBack != null)
                onRecvCallBack.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T: IPacket, new()
    {
        T pkt = new T();//4. PlayerInfoReq 이라는 패킷이 만들어지면서
        pkt.Read(buffer);
        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);// 5. 최종적으로  PacketHandler.PlayerInfoReqHandler를 호출하게 됌.
    }
}