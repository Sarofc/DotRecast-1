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
        public DtTileCacheLayerHeader Read(ref RcByteBuffer data)
        {
            DtTileCacheLayerHeader header = new DtTileCacheLayerHeader();
            header.magic = data.ReadInt32();
            header.version = data.ReadInt32();

            if (header.magic != DtTileCacheLayerHeader.DT_TILECACHE_MAGIC)
                throw new IOException("Invalid magic");
            if (header.version != DtTileCacheLayerHeader.DT_TILECACHE_VERSION)
                throw new IOException("Invalid version");

            header.tx = data.ReadInt32();
            header.ty = data.ReadInt32();
            header.tlayer = data.ReadInt32();

            header.bmin.X = data.ReadSingle();
            header.bmin.Y = data.ReadSingle();
            header.bmin.Z = data.ReadSingle();
            header.bmax.X = data.ReadSingle();
            header.bmax.Y = data.ReadSingle();
            header.bmax.Z = data.ReadSingle();

            header.hmin = data.ReadInt16() & 0xFFFF;
            header.hmax = data.ReadInt16() & 0xFFFF;
            header.width = data.ReadByte() & 0xFF;
            header.height = data.ReadByte() & 0xFF;
            header.minx = data.ReadByte() & 0xFF;
            header.maxx = data.ReadByte() & 0xFF;
            header.miny = data.ReadByte() & 0xFF;
            header.maxy = data.ReadByte() & 0xFF;

            return header;
        }
    }
}