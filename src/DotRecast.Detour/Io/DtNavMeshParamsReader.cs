using DotRecast.Core;

namespace DotRecast.Detour.Io
{
    public struct DtNavMeshParamsReader
    {
        public DtNavMeshParams Read(ref RcByteBuffer bb)
        {
            DtNavMeshParams option;
            option.orig.X = bb.ReadSingle();
            option.orig.Y = bb.ReadSingle();
            option.orig.Z = bb.ReadSingle();
            option.tileWidth = bb.ReadSingle();
            option.tileHeight = bb.ReadSingle();
            option.maxTiles = bb.ReadInt32();
            option.maxPolys = bb.ReadInt32();
            return option;
        }
    }
}