namespace DotRecast.Detour.TileCache
{
    public enum DtTileCacheObstacleType : byte
    {
        DT_OBSTACLE_CYLINDER,
        DT_OBSTACLE_BOX, // AABB
        DT_OBSTACLE_ORIENTED_BOX // OBB
    };
}