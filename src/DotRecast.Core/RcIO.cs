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

namespace DotRecast.Core
{
    public static class RcIO
    {
        public static Stream ReadFileIfFound(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;

            string filePath = filename;

            if (!File.Exists(filePath))
            {
                var searchFilePath = RcDirectory.SearchFile(filename);
                if (!File.Exists(searchFilePath))
                {
                    searchFilePath = RcDirectory.SearchFile($"resources/{filename}");
                }

                if (File.Exists(searchFilePath))
                {
                    filePath = searchFilePath;
                }
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return fs;
        }

        public static void Write(BinaryWriter bw, float value)
        {
            bw.Write(value);
        }

        public static void Write(BinaryWriter bw, short value)
        {
            bw.Write(value);
        }

        public static void Write(BinaryWriter ws, ushort value)
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

        public static void Write(BinaryWriter ws, uint value)
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
    }
}