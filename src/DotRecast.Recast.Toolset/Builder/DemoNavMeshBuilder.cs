using DotRecast.Detour;
using DotRecast.Recast.Geom;

namespace DotRecast.Recast.Toolset.Builder
{
    public static class DemoNavMeshBuilder
    {
        public static DtNavMeshCreateParams GetNavMeshCreateParams(IInputGeomProvider geom, float cellSize,
            float cellHeight, float agentHeight, float agentRadius, float agentMaxClimb,
            RcBuilderResult rcResult)
        {
            RcPolyMesh pmesh = rcResult.Mesh;
            RcPolyMeshDetail dmesh = rcResult.MeshDetail;
            DtNavMeshCreateParams option = new();
            for (int i = 0; i < pmesh.npolys; ++i)
            {
                pmesh.flags[i] = 1;
            }

            option.verts = pmesh.verts;
            option.vertCount = pmesh.nverts;
            option.polys = pmesh.polys;
            option.polyAreas = pmesh.areas;
            option.polyFlags = pmesh.flags;
            option.polyCount = pmesh.npolys;
            option.nvp = pmesh.nvp;
            if (dmesh != null)
            {
                option.detailMeshes = dmesh.meshes;
                option.detailVerts = dmesh.verts;
                option.detailVertsCount = dmesh.nverts;
                option.detailTris = dmesh.tris;
                option.detailTriCount = dmesh.ntris;
            }

            option.walkableHeight = agentHeight;
            option.walkableRadius = agentRadius;
            option.walkableClimb = agentMaxClimb;
            option.bmin = pmesh.bmin;
            option.bmax = pmesh.bmax;
            option.cs = cellSize;
            option.ch = cellHeight;
            option.buildBvTree = true;

            option.offMeshConCount = geom.OffMeshConCount;
            option.offMeshConVerts = geom.OffMeshConVerts;
            option.offMeshConRads = geom.OffMeshConRads;
            option.offMeshConDirs = geom.OffMeshConDirs;
            option.offMeshConAreas = geom.OffMeshConAreas;
            option.offMeshConFlags = geom.OffMeshConFlags;
            option.offMeshConUserID = geom.OffMeshConId;

            return option;
        }

        public static DtMeshData UpdateAreaAndFlags(DtMeshData meshData)
        {
            // Update poly flags from areas.
            for (int i = 0; i < meshData.polys.Length; ++i)
            {
                meshData.polys[i].flags = (ushort)(1 << meshData.polys[i].GetArea());
            }

            return meshData;
        }
    }
}