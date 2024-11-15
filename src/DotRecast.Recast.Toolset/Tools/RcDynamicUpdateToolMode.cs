using System.Collections.Immutable;

namespace DotRecast.Recast.Toolset.Tools
{
    public class RcDynamicUpdateToolMode
    {
        public static readonly RcDynamicUpdateToolMode BUILD = new RcDynamicUpdateToolMode(0, "Build");
        public static readonly RcDynamicUpdateToolMode COLLIDERS = new RcDynamicUpdateToolMode(1, "Colliders");
        public static readonly RcDynamicUpdateToolMode RAYCAST = new RcDynamicUpdateToolMode(2, "Raycast");

        public static readonly ImmutableArray<RcDynamicUpdateToolMode> Values = [
            BUILD, COLLIDERS, RAYCAST
        ];

        public readonly int Idx;
        public readonly string Label;

        private RcDynamicUpdateToolMode(int idx, string label)
        {
            Idx = idx;
            Label = label;
        }
    }
}