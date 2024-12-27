namespace DotRecast.Recast
{
    // Struct to keep track of entries in the region table that have been changed.
    public readonly struct RcDirtyEntry
    {
        public readonly int index;
        public readonly ushort region;
        public readonly ushort distance2;

        public RcDirtyEntry(int tempIndex, ushort tempRegion, ushort tempDistance2)
        {
            index = tempIndex;
            region = tempRegion;
            distance2 = tempDistance2;
        }
    }
}