using System;
using DotRecast.Core;

namespace DotRecast.Detour
{
    public struct DtCollectPolysQuery : IDtPolyQuery
    {
        private readonly long[] m_polys;
        private readonly int m_maxPolys;
        private int m_numCollected;
        private bool m_overflow;

        public DtCollectPolysQuery(long[] polys, int maxPolys)
        {
            m_polys = polys;
            m_maxPolys = maxPolys;
        }

        public readonly int NumCollected()
        {
            return m_numCollected;
        }

        public readonly bool Overflowed()
        {
            return m_overflow;
        }

        public void Process(DtMeshTile tile, DtPoly[] poly, ReadOnlySpan<long> refs, int count)
        {
            int numLeft = m_maxPolys - m_numCollected;
            int toCopy = count;
            if (toCopy > numLeft)
            {
                m_overflow = true;
                toCopy = numLeft;
            }

            RcSpans.Copy<long>(refs, 0, m_polys, m_numCollected, toCopy);
            m_numCollected += toCopy;
        }
    }
}