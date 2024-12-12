using System.Collections.Generic;

namespace DotRecast.Detour
{
    public class BVItemXComparer : IComparer<BVItem>
    {
        public static readonly BVItemXComparer Shared = new();

        private BVItemXComparer()
        {
        }

        public unsafe int Compare(BVItem a, BVItem b)
        {
            return a.bmin[0].CompareTo(b.bmin[0]);
        }
    }
}