using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using DotRecast.Core;
using DotRecast.Detour;
using DotRecast.Detour.TileCache;
using DotRecast.Detour.TileCache.Io;
using DotRecast.Recast.Geom;
using DotRecast.Recast.Toolset.Builder;
using DotRecast.Recast.Toolset.Geom;

namespace DotRecast.Recast.Toolset.Tools
{
    public class RcObstacleTool : IRcToolable
    {
        const int EXPECTED_LAYERS_PER_TILE = 4;

        private readonly IDtTileCacheMeshProcess _tmproc;
        private DtTileCache _tc;
        private IRcCompressor _compressor;

        public RcObstacleTool(IRcCompressor compressor) : this(compressor, new DemoDtTileCacheMeshProcess())
        { }

        public RcObstacleTool(IRcCompressor compressor, IDtTileCacheMeshProcess tmproc)
        {
            _compressor = compressor;
            _tmproc = tmproc;
        }

        public string GetName()
        {
            return "Temp Obstacles";
        }

        public NavMeshBuildResult Build(IInputGeomProvider geom, RcNavMeshBuildSettings setting)
        {
            if (null == geom || 0 == geom.Meshes().Count)
            {
                return new NavMeshBuildResult();
            }

            // TODO settings 
            //const int Threads = 1; // 1s
            int Threads = Environment.ProcessorCount;// 0.6s

            _tmproc.Init(geom);

            // Init cache
            var bmin = geom.GetMeshBoundsMin();
            var bmax = geom.GetMeshBoundsMax();
            RcRecast.CalcGridSize(bmin, bmax, setting.cellSize, out var gw, out var gh);
            int ts = setting.tileSize;
            int tw = (gw + ts - 1) / ts;
            int th = (gh + ts - 1) / ts;

            // Generation params.
            var walkableRadius = (int)MathF.Ceiling(setting.agentRadius / setting.cellSize); // Reserve enough padding.
            RcConfig cfg = new(
                true, setting.tileSize, setting.tileSize,
                walkableRadius + 3,
                setting.partitioning,
                setting.cellSize, setting.cellHeight,
                setting.agentMaxSlope, setting.agentHeight, setting.agentRadius, setting.agentMaxClimb,
                (int)RcMath.Sqr(setting.minRegionSize), (int)RcMath.Sqr(setting.mergedRegionSize), // Note: area = size*size
                (int)(setting.edgeMaxLen / setting.cellSize), setting.edgeMaxError,
                setting.vertsPerPoly,
                setting.detailSampleDist, setting.detailSampleMaxError,
                true, true, true,
                RcBuiltInAreas.POLYAREA_WALKABLE, true);

            var builder = new DtTileCacheLayerBuilder(LZ4Compressor.Shared);
            var results = builder.Build(geom, cfg, Threads, tw, th);
            var layers = results
                .SelectMany(x => x.layers)
                .ToList();

            _tc = CreateTileCache(geom, setting, tw, th);

            for (int i = 0; i < layers.Count; ++i)
            {
                var layer = layers[i];
                System.Diagnostics.Debug.Assert(layer.Length != 0);
                var refs = _tc.AddTile(layer, 0);
                _tc.BuildNavMeshTile(refs);
            }

            return new NavMeshBuildResult(ImmutableArray<RcBuilderResult>.Empty, _tc.GetNavMesh());
        }

        public bool Save(string file)
        {
            if (_tc == null)
                return false;

            var writer = new DtTileCacheWriter(_compressor);

            using var fs = new FileStream(file, FileMode.Create);
            using var bw = new BinaryWriter(fs);

            writer.Write(bw, _tc);

            // TODO convex volume 保存了，但是 link 没保存

            return true;
        }

        public void Load(string file)
        {
            var reader = new DtTileCacheReader(_compressor);

            using var fs = new FileStream(file, FileMode.Open);
            using var br = new BinaryReader(fs);

            _tc = reader.Read(br, 6, _tmproc);
        }

        public void ClearAllTempObstacles()
        {
            if (null == _tc)
                return;

            for (int i = 0; i < _tc.GetObstacleCount(); ++i)
            {
                DtTileCacheObstacle ob = _tc.GetObstacle(i);
                if (ob.state == DtObstacleState.DT_OBSTACLE_EMPTY)
                    continue;

                _tc.RemoveObstacle(_tc.GetObstacleRef(ob));
            }
        }

        public void RemoveTempObstacle(Vector3 sp, Vector3 sq)
        {
            if (null == _tc)
                return;

            long refs = HitTestObstacle(sp, sq);
            _tc.RemoveObstacle(refs);
        }

        public void RemoveObstacle(long refs)
        {
            if (null == _tc)
                return;

            _tc.RemoveObstacle(refs);
        }

        public long AddObstacle(Vector3 p, float raidus, float height)
        {
            if (null == _tc)
                return 0;

            p.Y -= 0.5f;
            return _tc.AddObstacle(p, raidus, height);
        }

        public long AddBoxObstacle(Vector3 bmin, Vector3 bmax)
        {
            if (null == _tc)
                return 0;

            return _tc.AddBoxObstacle(bmin, bmax);
        }

        public long AddBoxObstacle(Vector3 center, Vector3 extents, float yRadians)
        {
            if (null == _tc)
                return 0;

            return _tc.AddBoxObstacle(center, extents, yRadians);
        }

        public DtTileCache GetTileCache()
        {
            return _tc;
        }

        public DtTileCache CreateTileCache(IInputGeomProvider geom, RcNavMeshBuildSettings setting, int tw, int th)
        {
            DtTileCacheParams option = new();
            option.ch = setting.cellHeight;
            option.cs = setting.cellSize;
            option.orig = geom.GetMeshBoundsMin();
            option.height = setting.tileSize;
            option.width = setting.tileSize;
            option.walkableHeight = setting.agentHeight;
            option.walkableRadius = setting.agentRadius;
            option.walkableClimb = setting.agentMaxClimb;
            option.maxSimplificationError = setting.edgeMaxError;
            option.maxTiles = tw * th * EXPECTED_LAYERS_PER_TILE; // for test EXPECTED_LAYERS_PER_TILE;
            option.maxObstacles = 128;

            DtNavMeshParams navMeshParams = new();
            navMeshParams.orig = geom.GetMeshBoundsMin();
            navMeshParams.tileWidth = setting.tileSize * setting.cellSize;
            navMeshParams.tileHeight = setting.tileSize * setting.cellSize;

            navMeshParams.maxTiles = TileNavMeshBuilder.GetMaxTiles(geom, setting.cellSize, setting.tileSize, EXPECTED_LAYERS_PER_TILE);
            navMeshParams.maxPolys = TileNavMeshBuilder.GetMaxPolysPerTile(geom, setting.cellSize, setting.tileSize, EXPECTED_LAYERS_PER_TILE);
            //navMeshParams.maxTiles = 256; // ..
            //navMeshParams.maxPolys = 16384;

            var navMesh = new DtNavMesh();
            navMesh.Init(navMeshParams, 6);
            DtTileCache tc = new(option, navMesh, _compressor, _tmproc);
            return tc;
        }

        public long HitTestObstacle(Vector3 sp, Vector3 sq)
        {
            float tmin = float.MaxValue;
            DtTileCacheObstacle obmin = null;

            for (int i = 0; i < _tc.GetObstacleCount(); ++i)
            {
                DtTileCacheObstacle ob = _tc.GetObstacle(i);
                if (ob.state == DtObstacleState.DT_OBSTACLE_EMPTY)
                    continue;

                Vector3 bmin = Vector3.Zero;
                Vector3 bmax = Vector3.Zero;
                _tc.GetObstacleBounds(ob, ref bmin, ref bmax);

                if (RcIntersections.IsectSegAABB(sp, sq, bmin, bmax, out var t0, out var t1))
                {
                    if (t0 < tmin)
                    {
                        tmin = t0;
                        obmin = ob;
                    }
                }
            }

            return _tc.GetObstacleRef(obmin);
        }
    }
}