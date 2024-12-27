namespace DotRecast.Detour
{
    public unsafe struct BVItem
    {
        public fixed ushort bmin[3];
        public fixed ushort bmax[3];
        public int i;
    };
}