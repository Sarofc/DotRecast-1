namespace DotRecast.Recast
{
    public struct RcSweepSpan
    {
        public ushort rid; // row id
        public ushort id; // region id
        public ushort ns; // number samples
        public ushort nei; // neighbour id
    }
}