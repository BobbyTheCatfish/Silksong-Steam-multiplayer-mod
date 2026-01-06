using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace SilksongMultiplayer.NetworkData
{
    public static class PacketSerializer
    {
        // ---- Basic Types ----
        public static byte[] SerializeByte(byte value) =>
            new[] { value };

        public static byte[] SerializeBool(bool value) =>
            new[] { (byte)(value ? 1 : 0) };

        public static byte[] SerializeInt(int value) =>
            BitConverter.GetBytes(value); // 小端序

        public static byte[] SerializeFloat(float value) =>
            BitConverter.GetBytes(value);

        public static byte[] SerializeULong(ulong value) =>
            BitConverter.GetBytes(value);

        // ---- Composite Types ----
        public static byte[] SerializeString(string value)
        {
            if (value == null) value = string.Empty;
            byte[] strBytes = Encoding.UTF8.GetBytes(value);
            byte[] result = new byte[4 + strBytes.Length]; // 4-byte length + content
            byte[] lenBytes = BitConverter.GetBytes(strBytes.Length);
            Buffer.BlockCopy(lenBytes, 0, result, 0, 4);
            Buffer.BlockCopy(strBytes, 0, result, 4, strBytes.Length);
            return result;
        }

        public static byte[] SerializeVector2(Vector2 value)
        {
            byte[] xBytes = BitConverter.GetBytes(value.x);
            byte[] yBytes = BitConverter.GetBytes(value.y);
            byte[] result = new byte[xBytes.Length + yBytes.Length];
            Buffer.BlockCopy(xBytes, 0, result, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, result, xBytes.Length, yBytes.Length);
            return result;
        }

        public static byte[] SerializeVector3(Vector3 value)
        {
            byte[] xBytes = BitConverter.GetBytes(value.x);
            byte[] yBytes = BitConverter.GetBytes(value.y);
            byte[] zBytes = BitConverter.GetBytes(value.z);
            byte[] result = new byte[xBytes.Length + yBytes.Length + zBytes.Length];
            Buffer.BlockCopy(xBytes, 0, result, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, result, xBytes.Length, yBytes.Length);
            Buffer.BlockCopy(zBytes, 0 , result, xBytes.Length + yBytes.Length, zBytes.Length);
            return result;
        }

        // ---- Concatenate multiple byte arrays ----
        public static byte[] Combine(params byte[][] arrays)
        {
            int totalLength = 0;
            foreach (var arr in arrays)
                totalLength += arr.Length;

            byte[] result = new byte[totalLength];
            int offset = 0;
            foreach (var arr in arrays)
            {
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }
    }

}
