using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name;

        public struct skillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> s, ref ushort count)
            {
                bool success = true;
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), id);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), level);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), duration);
                count += sizeof(float);
                return success;
            }

            public void Read(ReadOnlySpan<byte> s, ref ushort count)
            {
                id = BitConverter.ToInt32(s.Slice(count, s.Length - count));
                count += sizeof(int);
                level = BitConverter.ToInt16(s.Slice(count, s.Length - count));
                count += sizeof(short);
                duration = BitConverter.ToSingle(s.Slice(count, s.Length - count));
                count += sizeof(float);
            }
        }
        public List<skillInfo> skills = new List<skillInfo>();

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += sizeof(ushort);
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(s.Slice(count, s.Length - count));
            count += sizeof(long);

            //string
            ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(count, nameLen));
            count += nameLen;

            //skill
            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
            count += sizeof(ushort);
            for (int i = 0; i < skillLen; i++)
            {
                skillInfo skill = new skillInfo();
                skill.Read(s, ref count);
                skills.Add(skill);
            }
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
            //[ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
            //success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, s.Count), packet.size);
            // size는 아래의 연산이 다끝난 후에 알 수 있으므로 마지막에 연산을 하게 바꾼다.
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.playerId);
            count += sizeof(long);

            // string을 생으로 바이트로 변환하지말고
            // string len [2]  스트링렌스를 2바이트짜리로 보낸다음에 (크기는 얼마짜리다)
            // byte[] 해당하는 크기의 데이터로 보내자

            //this.name.Length; "abcd"인 경우 문자열의길이는 4이지만 실제로 바이트배열로 변환하면 8바이트가 나오게 됨
            // 그렇기 때문에 length를 사용할 수 없고 utf-16을 사용
            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            //success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);// 몇바이트가 갈지 미리 보내줌
            //count += sizeof(ushort);
            //Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            //count += nameLen;

            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort)/*미리 공간확보*/);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            //skill list
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)skills.Count);// 크기를 먼저 밀어준다.
            count += sizeof(ushort);
            foreach (skillInfo skill in skills)
            {
                //TODO
                success &= skill.Write(s, ref count);
            }


            success &= BitConverter.TryWriteBytes(s, count); // size 패킷(최종 길이)

            if (success == false)
                return null;

            return SendBufferHelper.Close(count);

        }
    }

        public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }
    public class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnConnected : {endpoint}");
            //Packet packet = new Packet(){ size = 100, packetId = 10};
            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);//

            //Send(openSegment);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerinfoReq: {p.playerId}, {p.name}");
                        foreach (PlayerInfoReq.skillInfo skill in p.skills)
                        {
                            Console.WriteLine($"skill id({skill.id}), level({skill.level}), duration({skill.duration})");
                        }
                    }
                    break;
            }
            Console.WriteLine($"RecvPacketId : {id}, Size : {size}");
        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnDisconnected : {endpoint}");

        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");

        }
    }
}