﻿// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Runtime.InteropServices;

namespace Black0ut.Byte
{
    public static class BytePro
    {
        #region Equality

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static bool Equals(byte[] a, byte[] b)
        {
            return a.Length == b.Length && memcmp(a, b, a.Length) == 0;
        }

        #endregion

        #region Set

        public static byte[] Set(byte[] data, ref int index, params byte[][] datas)
        {
            for (int i = 0, count = datas.Length; i < count; i++)
            {
                Buffer.BlockCopy(datas[i], 0, data, index, datas[i].Length);

                index += datas[i].Length;
            }

            return data;
        }

        public static byte[] Set(byte[] data, int index, params byte[][] datas)
        {
            return Set(data, ref index, datas);
        }

        #endregion

        #region Combine

        public static byte[] Combine(params byte[][] datas)
        {
            var dataLength = 0;

            for (int i = 0, count = datas.Length; i < count; i++)
            {
                dataLength += datas[i].Length;
            }

            return Set(new byte[dataLength], 0, datas);
        }

        public static byte[] Combine(byte firstByte, params byte[][] datas)
        {
            var dataLength = 1;

            for (int i = 0, count = datas.Length; i < count; i++)
            {
                dataLength += datas[i].Length;
            }

            var data = Set(new byte[dataLength], 1, datas);

            data[0] = firstByte;

            return data;
        }

        #endregion
    }
}