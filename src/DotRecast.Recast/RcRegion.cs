using System.Collections.Generic;

namespace DotRecast.Recast
{
    public class RcRegion
    {
        public int spanCount; // Number of spans belonging to this region
        public ushort id; // ID of the region
        public byte areaType; // Are type.
        public bool remap;
        public bool visited;
        public bool overlap;
        public bool connectsToBorder;
        public ushort ymin, ymax;
        public List<int> connections;
        public List<int> floors;

        public RcRegion(ushort i)
        {
            id = i;
            ymin = 0xFFFF;
            connections = new List<int>();
            floors = new List<int>();
        }
    }
}