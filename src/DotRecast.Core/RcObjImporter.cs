/*
recast4j Copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org
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
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cysharp.IO;

namespace DotRecast.Core
{
    public static class RcObjImporter
    {
        public static RcObjImporterContext LoadContext(Stream stream)
        {
            var context = new RcObjImporterContext();

            // 达成字符串解析0gc！

            const int BUFFER_SIZE = 4096;
            using var reader = new Utf8StreamReader(stream)
            {
                SyncRead = true // 同步读取
            }
            .AsTextReader(BUFFER_SIZE);
#pragma warning disable
            while (reader.LoadIntoBufferAsync().Result) // SyncRead 可以直接用Result，而不需要等
#pragma warning restore
            {
                while (reader.TryReadLine(out var line))
                {
                    ReadLine(line.Span, ref context);
                }
            }

            return context;
        }

        public static void ReadLine(ReadOnlySpan<char> line, ref RcObjImporterContext context)
        {
            line = line.Trim();
            if (line.StartsWith("v"))
            {
                ReadVertex(line, ref context);
            }
            else if (line.StartsWith("f"))
            {
                ReadFace(line, ref context);
            }
        }

        private static void ReadVertex(ReadOnlySpan<char> line, ref RcObjImporterContext context)
        {
            if (line.StartsWith("v "))
            {
                var vert = ReadVector3f(line);
                context.AddVertex(vert.X);
                context.AddVertex(vert.Y);
                context.AddVertex(vert.Z);
            }
        }

        [SkipLocalsInit]
        private static Vector3 ReadVector3f(ReadOnlySpan<char> line)
        {
            Span<Range> v = stackalloc Range[4];
            var n = line.Split(v, ' ', StringSplitOptions.RemoveEmptyEntries);
            if (n < 4)
            {
                throw new Exception("Invalid vector, expected 3 coordinates, found " + (n - 1));
            }

            // fix - https://github.com/ikpil/DotRecast/issues/7
            return new Vector3(
                float.Parse(line[v[1]], CultureInfo.InvariantCulture),
                float.Parse(line[v[2]], CultureInfo.InvariantCulture),
                float.Parse(line[v[3]], CultureInfo.InvariantCulture)
            );
        }

        [SkipLocalsInit]
        private static void ReadFace(ReadOnlySpan<char> line, ref RcObjImporterContext context)
        {
            Span<Range> v = stackalloc Range[16]; // incase
            var n = line.Split(v, ' ', StringSplitOptions.RemoveEmptyEntries);
            if (n < 4)
            {
                throw new Exception("Invalid number of face vertices: 3 coordinates expected, found " + n);
            }

            for (int j = 0; j < n - 3; j++)
            {
                context.AddFace(ReadFaceVertex(line[v[1]], ref context));
                for (int i = 0; i < 2; i++)
                {
                    context.AddFace(ReadFaceVertex(line[v[2 + j + i]], ref context));
                }
            }
        }

        [SkipLocalsInit]
        private static int ReadFaceVertex(ReadOnlySpan<char> face, ref RcObjImporterContext context)
        {
            Span<Range> v = stackalloc Range[2];
            var n = face.Split(v, '/');
            return GetIndex(int.Parse(face[v[0]]), context.Vertices.Length);
        }

        private static int GetIndex(int posi, int size)
        {
            if (posi > 0)
            {
                posi--;
            }
            else if (posi < 0)
            {
                posi = size + posi;
            }
            else
            {
                throw new Exception("0 vertex index");
            }

            return posi;
        }
    }
}