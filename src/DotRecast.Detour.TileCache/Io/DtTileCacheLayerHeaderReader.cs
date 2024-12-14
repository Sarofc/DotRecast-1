/*
Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
recast4j copyright (c) 2015-2019 Piotr Piastucki piotr@jtilia.org
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

using System.IO;
using DotRecast.Core;

namespace DotRecast.Detour.TileCache.Io
{
    public struct DtTileCacheLayerHeaderReader
    {
        public DtTileCacheLayerHeader Read(ref RcByteBuffer br)
        {
            DtTileCacheLayerHeader header = new();
            header.magic = br.ReadInt32();
            header.version = br.ReadInt32();

            if (header.magic != DtTileCacheLayerHeader.DT_TILECACHE_MAGIC)
                throw new IOException("Invalid magic");
            if (header.version != DtTileCacheLayerHeader.DT_TILECACHE_VERSION)
                throw new IOException("Invalid version");

            header.tx = br.ReadInt32();
            header.ty = br.ReadInt32();
            header.tlayer = br.ReadInt32();

            header.bmin.X = br.ReadSingle();
            header.bmin.Y = br.ReadSingle();
            header.bmin.Z = br.ReadSingle();
            header.bmax.X = br.ReadSingle();
            header.bmax.Y = br.ReadSingle();
            header.bmax.Z = br.ReadSingle();

            header.hmin = br.ReadInt16() & 0xFFFF;
            header.hmax = br.ReadInt16() & 0xFFFF;
            header.width = br.ReadByte() & 0xFF;
            header.height = br.ReadByte() & 0xFF;
            header.minx = br.ReadByte() & 0xFF;
            header.maxx = br.ReadByte() & 0xFF;
            header.miny = br.ReadByte() & 0xFF;
            header.maxy = br.ReadByte() & 0xFF;

            return header;
        }
    }
}