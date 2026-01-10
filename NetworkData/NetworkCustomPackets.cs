using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SilksongMultiplayer.NetworkData
{
    public class NetworkCustomPacket
    {
        internal byte packetNum;
        internal Action<byte[], CSteamID, int> receiveHandler;
        public NetworkCustomPacket(byte packetID, Action<byte[], CSteamID, int> receiveHandler)
        {
            packetNum = packetID;
            this.receiveHandler = receiveHandler;
        }

        public void SendPacket(byte[] data, EP2PSend sendType = EP2PSend.k_EP2PSendReliable)
        {
            Debug.Log("Sending packet");
            data = PacketSerializer.Combine(
                    PacketSerializer.SerializeByte(packetNum),
                    data
            );

            NetworkDataSender.Broadcast(data, sendType);
        }

        internal void PacketHandler(byte[] data, CSteamID senderID, int offset)
        {
            receiveHandler.Invoke(data, senderID, offset);
        }
    }
}
