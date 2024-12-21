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
    public struct DtTileCacheLayerHeaderWriter
    {
        public void Write(RcSpanWriter sw, in DtTileCacheLayerHeader header)
        {
            sw.Write(header.magic);
            sw.Write(header.version);
            sw.Write(header.tx);
            sw.Write(header.ty);
            sw.Write(header.tlayer);

            sw.Write(header.bmin.X);
            sw.Write(header.bmin.Y);
            sw.Write(header.bmin.Z);
            sw.Write(header.bmax.X);
            sw.Write(header.bmax.Y);
            sw.Write(header.bmax.Z);

            sw.Write((short)header.hmin);
            sw.Write((short)header.hmax);
            sw.Write((byte)header.width);
            sw.Write((byte)header.height);
            sw.Write((byte)header.minx);
            sw.Write((byte)header.maxx);
            sw.Write((byte)header.miny);
            sw.Write((byte)header.maxy);
        }
    }
}