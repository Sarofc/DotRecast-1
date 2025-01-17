using DotRecast.Detour;
using DotRecast.Detour.TileCache;
using DotRecast.Recast.Geom;

namespace DotRecast.Recast.Toolset.Geom
{
    public class DemoDtTileCacheMeshProcess : IDtTileCacheMeshProcess
    {
        private IInputGeomProvider m_geom;

        public DemoDtTileCacheMeshProcess()
        {
        }

        public void Init(IInputGeomProvider geom)
        {
            m_geom = geom;
        }

        public void Process(DtNavMeshCreateParams option)
        {
            // Update poly flags from areas.
            for (int i = 0; i < option.polyCount; ++i)
            {
                option.polyFlags[i] = (ushort)(1 << option.polyAreas[i]);
            }

            // Pass in off-mesh connections.
            if (null != m_geom)
            {
                option.offMeshConCount = m_geom.OffMeshConCount;
                option.offMeshConVerts = m_geom.OffMeshConVerts;
                option.offMeshConRads = m_geom.OffMeshConRads;
                option.offMeshConDirs = m_geom.OffMeshConDirs;
                option.offMeshConAreas = m_geom.OffMeshConAreas;
                option.offMeshConFlags = m_geom.OffMeshConFlags;
                option.offMeshConUserID = m_geom.OffMeshConId;
            }
        }
    }
}