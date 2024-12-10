/*
Recast4J Copyright (c) 2015 Piotr Piastucki piotr@jtilia.org
DotRecast Copyright (c) 2023-2024 Choi Ikpil ikpil@naver.com

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
 claim that you wrote the original software. If you use this software
 in a product, an acknowledgment in the product documentation would be
 appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
 misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace DotRecast.Core
{
    public static class RcIO
    {
        public static RcByteBuffer ToByteBuffer(BinaryReader br, bool direct)
        {
            byte[] data = ToByteArray(br);
            if (direct)
            {
                Array.Reverse(data);
            }

            return new RcByteBuffer(data);
        }

        [SkipLocalsInit]
        public static byte[] ToByteArray(BinaryReader br)
        {
            using var ms = new MemoryStream();
            Span<byte> buffer = stackalloc byte[4096];
            int l;
            while ((l = br.Read(buffer)) > 0)
            {
                ms.Write(buffer.Slice(0, l));
            }

            return ms.ToArray();
        }


        public static RcByteBuffer ToByteBuffer(BinaryReader br)
        {
            var bytes = ToByteArray(br);
            return new RcByteBuffer(bytes);
        }

        public static byte[] ReadFileIfFound(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;

            string filePath = filename;

            if (!File.Exists(filePath))
            {
                var searchFilePath = RcDirectory.SearchFile($"{filename}");
                if (!File.Exists(searchFilePath))
                {
                    searchFilePath = RcDirectory.SearchFile($"resources/{filename}");
                }

                if (File.Exists(searchFilePath))
                {
                    filePath = searchFilePath;
                }
            }

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] buffer = new byte[fs.Length];
            var read = fs.Read(buffer, 0, buffer.Length);
            if (read != buffer.Length)
                return null;

            return buffer;
        }

        public static void Write(BinaryWriter ws, float value)
        {
            ws.Write(value);
        }

        public static void Write(BinaryWriter ws, short value)
        {
            ws.Write(value);
        }

        public static void Write(BinaryWriter ws, long value)
        {
            ws.Write(value);
        }

        public static void Write(BinaryWriter ws, int value)
        {
            ws.Write(value);
        }

        public static void Write(BinaryWriter ws, bool value)
        {
            ws.Write(value);
        }

        public static void Write(BinaryWriter ws, byte value)
        {
            ws.Write(value);
        }

        //public static void Write(BinaryWriter ws, MemoryStream ms)
        //{
        //    ms.Position = 0;
        //    byte[] buffer = new byte[ms.Length];
        //    ms.Read(buffer, 0, buffer.Length);
        //    ws.Write(buffer);
        //}
    }
}