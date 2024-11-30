using System.Numerics;

namespace DotRecast.Detour.Crowd
{
    public struct DtSegment
    {
        /** Segment start/end */
        public Vector3 s, e;

        /** Distance for pruning. */
        public float d;
    }
}