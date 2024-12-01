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
        public void Write(BinaryWriter stream, DtTileCacheLayerHeader header)
        {
            RcIO.Write(stream, header.magic);
            RcIO.Write(stream, header.version);
            RcIO.Write(stream, header.tx);
            RcIO.Write(stream, header.ty);
            RcIO.Write(stream, header.tlayer);

            RcIO.Write(stream, header.bmin.X);
            RcIO.Write(stream, header.bmin.Y);
            RcIO.Write(stream, header.bmin.Z);
            RcIO.Write(stream, header.bmax.X);
            RcIO.Write(stream, header.bmax.Y);
            RcIO.Write(stream, header.bmax.Z);

            RcIO.Write(stream, (short)header.hmin);
            RcIO.Write(stream, (short)header.hmax);
            RcIO.Write(stream, (byte)header.width);
            RcIO.Write(stream, (byte)header.height);
            RcIO.Write(stream, (byte)header.minx);
            RcIO.Write(stream, (byte)header.maxx);
            RcIO.Write(stream, (byte)header.miny);
            RcIO.Write(stream, (byte)header.maxy);
        }
    }
}