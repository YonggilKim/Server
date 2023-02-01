using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;

        Console.WriteLine($"C_PlayerInfoReq: {p.playerId}, {p.name}");

        foreach (C_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"skill id({skill.id}), level({skill.level}), duration({skill.duration})");
        }
    }

    public static void TestHandler(PacketSession session, IPacket packet)
    {

    }
}

