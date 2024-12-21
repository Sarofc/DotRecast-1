using System;
using System.Numerics;
using DotRecast.Detour.TileCache;
using DotRecast.Recast.Demo.Draw;
using DotRecast.Recast.Toolset;
using DotRecast.Recast.Toolset.Tools;
using ImGuiNET;
using Serilog;

namespace DotRecast.Recast.Demo.Tools;

public class ObstacleSampleTool : ISampleTool
{
    private static readonly ILogger Logger = Log.ForContext<ObstacleSampleTool>();

    private DemoSample _sample;
    private readonly RcObstacleTool _tool;

    public ObstacleSampleTool()
    {
        _tool = new(LZ4Compressor.Shared);
    }

    public void Layout()
    {
        if (ImGui.Button("Build Tile Cache"))
        {
            var geom = _sample.GetInputGeom();
            var settings = _sample.GetSettings();

            var buildResult = _tool.Build(geom, settings);
            if (buildResult.Success)
            {
                _sample.Update(_sample.GetInputGeom(), buildResult.RecastBuilderResults, buildResult.NavMesh);
                Console.WriteLine(buildResult.NavMesh.ToString());
            }

            GC.Collect();
        }

        if (ImGui.Button("Remove All Temp Obstacles"))
        {
            _tool.ClearAllTempObstacles();
        }

        if (ImGui.Button("Save Tile Cache"))
        {
            var success = _tool.Save("sample.tilecache");
            if (success)
            {
                Logger.Information("Save Tile Cache success");
            }
        }

        if (ImGui.Button("Load Tile Cache"))
        {
            _tool.Load("sample.tilecache");
            _sample.Update(_sample.GetInputGeom(), Array.Empty<RcBuilderResult>(), _tool.GetTileCache().GetNavMesh());
        }

        ImGui.Separator();

        ImGui.Text("Click LMB to create an obstacle.");
        ImGui.Text("Shift+LMB to remove an obstacle.");
    }

    public void HandleRender(NavMeshRenderer renderer)
    {
        DrawObstacles(renderer.GetDebugDraw());
    }


    private void DrawObstacles(RecastDebugDraw dd)
    {
        var tc = _tool.GetTileCache();
        if (null == tc)
            return;

        // Draw obstacles
        for (int i = 0; i < tc.GetObstacleCount(); ++i)
        {
            var ob = tc.GetObstacle(i);
            if (ob.state == DtObstacleState.DT_OBSTACLE_EMPTY)
                continue;

            Vector3 bmin = Vector3.Zero;
            Vector3 bmax = Vector3.Zero;
            tc.GetObstacleBounds(ob, ref bmin, ref bmax);

            int col = 0;
            if (ob.state == DtObstacleState.DT_OBSTACLE_PROCESSING)
                col = DebugDraw.DuRGBA(255, 255, 0, 128);
            else if (ob.state == DtObstacleState.DT_OBSTACLE_PROCESSED)
                col = DebugDraw.DuRGBA(255, 192, 0, 192);
            else if (ob.state == DtObstacleState.DT_OBSTACLE_REMOVING)
                col = DebugDraw.DuRGBA(220, 0, 0, 128);

            switch (ob.type)
            {
                case DtTileCacheObstacleType.DT_OBSTACLE_CYLINDER:
                    {
                        dd.DebugDrawCylinder(bmin.X, bmin.Y, bmin.Z, bmax.X, bmax.Y, bmax.Z, col);
                        dd.DebugDrawCylinderWire(bmin.X, bmin.Y, bmin.Z, bmax.X, bmax.Y, bmax.Z, DebugDraw.DuDarkenCol(col), 2);
                    }
                    break;
                case DtTileCacheObstacleType.DT_OBSTACLE_BOX:
                case DtTileCacheObstacleType.DT_OBSTACLE_ORIENTED_BOX: // TODO obb 没有绘制函数，以后再说
                    {
                        dd.DebugDrawBox(bmin.X, bmin.Y, bmin.Z, bmax.X, bmax.Y, bmax.Z, col);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public IRcToolable GetTool()
    {
        return _tool;
    }

    public void SetSample(DemoSample sample)
    {
        _sample = sample;
    }

    public void OnSampleChanged()
    {
    }


    public void HandleClick(Vector3 s, Vector3 p, bool shift)
    {
        if (shift)
        {
            _tool.RemoveTempObstacle(s, p);
        }
        else
        {
            _tool.AddObstacle(p, 1.0f, 2.0f);
        }
    }


    public void HandleUpdate(float dt)
    {
        var tc = _tool.GetTileCache();
        if (null != tc)
            tc.Update();
    }

    public void HandleClickRay(Vector3 start, Vector3 direction, bool shift)
    {
    }
}