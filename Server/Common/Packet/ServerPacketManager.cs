using ServerCore;
using System.Collections.Generic;
using System;

class PacketManager
{
    #region Singleton
    static PacketManager _instance;
    public static PacketManager Instance
    {
        get
        {
            if(_instance == null)
                _instance = new PacketManager();
            return _instance;
        }
    }
    #endregion

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {
        _onRecv.Add((ushort)PacketID.C_PlayerInfoReq, MakePacket<C_PlayerInfoReq>);
        _handler.Add((ushort)PacketID.C_PlayerInfoReq, PacketHandler.C_PlayerInfoReqHandler);


    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer) 
    {
        //2. 사이즈와 아이디를 가지고와서 switch-case문에서 하던걸 -> 딕셔너리로 가져옴
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
            
        //3. 실제로 핸들러를 등록해 놓으면 그곳에 인보크를 할 예정
        Action<PacketSession, ArraySegment<byte>> action = null;
        if(_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);// 호출되는 순간 Makepacket 호출 -> Register()에서 MacketPacket을 등록해놨으니깐.
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T: IPacket, new()
    {
        T pkt = new T();//4. PlayerInfoReq 이라는 패킷이 만들어지면서
        pkt.Read(buffer);
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);// 5. 최종적으로  PacketHandler.PlayerInfoReqHandler를 호출하게 됌.
    }
}