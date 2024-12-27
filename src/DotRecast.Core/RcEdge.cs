namespace DotRecast.Core
{
    public unsafe struct RcEdge
    {
        public fixed ushort vert[2];
        public fixed ushort polyEdge[2];
        public fixed ushort poly[2];
    }
}