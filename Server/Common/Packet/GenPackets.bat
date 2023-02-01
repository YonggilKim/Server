START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../DummyClient/Packet"
XCOPY /Y GenPackets.cs "../../Server/Packet"
XCOPY /Y ClientPacketPacketManager.cs "../../Server/Packet"
XCOPY /Y ServerPacketPacketManager.cs "../../Server/Packet"

