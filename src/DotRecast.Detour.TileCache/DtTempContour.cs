using System;

namespace DotRecast.Detour.TileCache
{
    public ref struct DtTempContour
    {
        public readonly Span<byte> verts;
        public readonly Span<ushort> poly;
        public readonly int cverts;
        public int nverts;
        public int npoly;

        public DtTempContour(Span<byte> verts, Span<ushort> poly)
        {
            System.Diagnostics.Debug.Assert(verts.Length % 4 == 0);

            this.verts = verts;
            this.poly = poly;
            cverts = verts.Length / 4;
            nverts = 0;
            npoly = 0;
        }
    };
}