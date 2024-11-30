using System.IO;
using DotRecast.Core;
using System.Numerics;

namespace DotRecast.Detour.Io
{
    public struct DtNavMeshParamWriter
    {
        public void Write(BinaryWriter stream, DtNavMeshParams option)
        {
            RcIO.Write(stream, option.orig.X);
            RcIO.Write(stream, option.orig.Y);
            RcIO.Write(stream, option.orig.Z);
            RcIO.Write(stream, option.tileWidth);
            RcIO.Write(stream, option.tileHeight);
            RcIO.Write(stream, option.maxTiles);
            RcIO.Write(stream, option.maxPolys);
        }
    }
}