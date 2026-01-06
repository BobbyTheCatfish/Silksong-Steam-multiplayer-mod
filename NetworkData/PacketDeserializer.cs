using System;
using System.Text;
using UnityEngine;

namespace SilksongMultiplayer.NetworkData
{
    public static class PacketDeserializer
    {
        public static byte ReadByte(byte[] data, ref int offset)
        {
            return data[offset++];
        }

        public static bool ReadBool(byte[] data, ref int offset)
        {
            return data[offset++] != 0;
        }

        public static int ReadInt(byte[] data, ref int offset)
        {
            int value = BitConverter.ToInt32(data, offset);
            offset += 4;
            return value;
        }

        public static float ReadFloat(byte[] data, ref int offset)
        {
            float value = BitConverter.ToSingle(data, offset);
            offset += 4;
            return value;
        }

        public static ulong ReadULong(byte[] data, ref int offset)
        {
            ulong value = BitConverter.ToUInt64(data, offset);
            offset += 8;
            return value;
        }

        public static string ReadString(byte[] data, ref int offset)
        {
            // Read the first 4 bytes (which represent the length).
            int length = BitConverter.ToInt32(data, offset);
            offset += 4;

            string value = Encoding.UTF8.GetString(data, offset, length);
            offset += length;
            return value;
        }

        public static Vector2 ReadVector2(byte[] data, ref int offset)
        {
            float x = ReadFloat(data, ref offset);
            float y = ReadFloat(data, ref offset);
            return new Vector2(x, y);
        }

        public static Vector3 ReadVector3(byte[] data, ref int offset)
        {
            float x = ReadFloat(data, ref offset);
            float y = ReadFloat(data, ref offset);
            float z = ReadFloat(data, ref offset);
            return new Vector3(x, y, z);
        }
    }

}
