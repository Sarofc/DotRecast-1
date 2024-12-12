namespace DotRecast.Core
{
    /// Recast performance timer categories.
    /// @see rcContext
    public static class RcTimerLabel
    {
        /// The user defined total time of the build.
        public static readonly string RC_TIMER_TOTAL = nameof(RC_TIMER_TOTAL);

        /// A user defined build time.
        public static readonly string RC_TIMER_TEMP = nameof(RC_TIMER_TEMP);

        /// The time to rasterize the triangles. (See: #rcRasterizeTriangle)
        public static readonly string RC_TIMER_RASTERIZE_TRIANGLES = nameof(RC_TIMER_RASTERIZE_TRIANGLES);
        public static readonly string RC_TIMER_RASTERIZE_SPHERE = nameof(RC_TIMER_RASTERIZE_SPHERE);
        public static readonly string RC_TIMER_RASTERIZE_CAPSULE = nameof(RC_TIMER_RASTERIZE_CAPSULE);
        public static readonly string RC_TIMER_RASTERIZE_CYLINDER = nameof(RC_TIMER_RASTERIZE_CYLINDER);
        public static readonly string RC_TIMER_RASTERIZE_BOX = nameof(RC_TIMER_RASTERIZE_BOX);
        public static readonly string RC_TIMER_RASTERIZE_CONVEX = nameof(RC_TIMER_RASTERIZE_CONVEX);

        /// The time to build the compact heightfield. (See: #rcBuildCompactHeightfield)
        public static readonly string RC_TIMER_BUILD_COMPACTHEIGHTFIELD = nameof(RC_TIMER_BUILD_COMPACTHEIGHTFIELD);

        /// The total time to build the contours. (See: #rcBuildContours)
        public static readonly string RC_TIMER_BUILD_CONTOURS = nameof(RC_TIMER_BUILD_CONTOURS);

        /// The time to trace the boundaries of the contours. (See: #rcBuildContours)
        public static readonly string RC_TIMER_BUILD_CONTOURS_TRACE = nameof(RC_TIMER_BUILD_CONTOURS_TRACE);

        public static readonly string RC_TIMER_BUILD_CONTOURS_WALK = nameof(RC_TIMER_BUILD_CONTOURS_WALK);

        /// The time to simplify the contours. (See: #rcBuildContours)
        public static readonly string RC_TIMER_BUILD_CONTOURS_SIMPLIFY = nameof(RC_TIMER_BUILD_CONTOURS_SIMPLIFY);

        /// The time to filter ledge spans. (See: #rcFilterLedgeSpans)
        public static readonly string RC_TIMER_FILTER_BORDER = nameof(RC_TIMER_FILTER_BORDER);

        /// The time to filter low height spans. (See: #rcFilterWalkableLowHeightSpans)
        public static readonly string RC_TIMER_FILTER_WALKABLE = nameof(RC_TIMER_FILTER_WALKABLE);

        /// The time to apply the median filter. (See: #rcMedianFilterWalkableArea)
        public static readonly string RC_TIMER_MEDIAN_AREA = nameof(RC_TIMER_MEDIAN_AREA);

        /// The time to filter low obstacles. (See: #rcFilterLowHangingWalkableObstacles)
        public static readonly string RC_TIMER_FILTER_LOW_OBSTACLES = nameof(RC_TIMER_FILTER_LOW_OBSTACLES);

        /// The time to build the polygon mesh. (See: #rcBuildPolyMesh)
        public static readonly string RC_TIMER_BUILD_POLYMESH = nameof(RC_TIMER_BUILD_POLYMESH);

        /// The time to merge polygon meshes. (See: #rcMergePolyMeshes)
        public static readonly string RC_TIMER_MERGE_POLYMESH = nameof(RC_TIMER_MERGE_POLYMESH);

        /// The time to erode the walkable area. (See: #rcErodeWalkableArea)
        public static readonly string RC_TIMER_ERODE_AREA = nameof(RC_TIMER_ERODE_AREA);

        /// The time to mark a box area. (See: #rcMarkBoxArea)
        public static readonly string RC_TIMER_MARK_BOX_AREA = nameof(RC_TIMER_MARK_BOX_AREA);

        /// The time to mark a cylinder area. (See: #rcMarkCylinderArea)
        public static readonly string RC_TIMER_MARK_CYLINDER_AREA = nameof(RC_TIMER_MARK_CYLINDER_AREA);

        /// The time to mark a convex polygon area. (See: #rcMarkConvexPolyArea)
        public static readonly string RC_TIMER_MARK_CONVEXPOLY_AREA = nameof(RC_TIMER_MARK_CONVEXPOLY_AREA);

        /// The total time to build the distance field. (See: #rcBuildDistanceField)
        public static readonly string RC_TIMER_BUILD_DISTANCEFIELD = nameof(RC_TIMER_BUILD_DISTANCEFIELD);

        /// The time to build the distances of the distance field. (See: #rcBuildDistanceField)
        public static readonly string RC_TIMER_BUILD_DISTANCEFIELD_DIST = nameof(RC_TIMER_BUILD_DISTANCEFIELD_DIST);

        /// The time to blur the distance field. (See: #rcBuildDistanceField)
        public static readonly string RC_TIMER_BUILD_DISTANCEFIELD_BLUR = nameof(RC_TIMER_BUILD_DISTANCEFIELD_BLUR);

        /// The total time to build the regions. (See: #rcBuildRegions, #rcBuildRegionsMonotone)
        public static readonly string RC_TIMER_BUILD_REGIONS = nameof(RC_TIMER_BUILD_REGIONS);

        /// The total time to apply the watershed algorithm. (See: #rcBuildRegions)
        public static readonly string RC_TIMER_BUILD_REGIONS_WATERSHED = nameof(RC_TIMER_BUILD_REGIONS_WATERSHED);

        /// The time to expand regions while applying the watershed algorithm. (See: #rcBuildRegions)
        public static readonly string RC_TIMER_BUILD_REGIONS_EXPAND = nameof(RC_TIMER_BUILD_REGIONS_EXPAND);

        /// The time to flood regions while applying the watershed algorithm. (See: #rcBuildRegions)
        public static readonly string RC_TIMER_BUILD_REGIONS_FLOOD = nameof(RC_TIMER_BUILD_REGIONS_FLOOD);

        /// The time to filter out small regions. (See: #rcBuildRegions, #rcBuildRegionsMonotone)
        public static readonly string RC_TIMER_BUILD_REGIONS_FILTER = nameof(RC_TIMER_BUILD_REGIONS_FILTER);

        /// The time to build heightfield layers. (See: #rcBuildHeightfieldLayers)
        public static readonly string RC_TIMER_BUILD_LAYERS = nameof(RC_TIMER_BUILD_LAYERS);

        /// The time to build the polygon mesh detail. (See: #rcBuildPolyMeshDetail)
        public static readonly string RC_TIMER_BUILD_POLYMESHDETAIL = nameof(RC_TIMER_BUILD_POLYMESHDETAIL);

        /// The time to merge polygon mesh details. (See: #rcMergePolyMeshDetails)
        public static readonly string RC_TIMER_MERGE_POLYMESHDETAIL = nameof(RC_TIMER_MERGE_POLYMESHDETAIL);


        /// The maximum number of timers.  (Used for iterating timers.)
        public static readonly string RC_MAX_TIMERS = nameof(RC_MAX_TIMERS);
    };
}