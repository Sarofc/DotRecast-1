namespace DotRecast.Detour.Crowd
{
    public static class DtCrowdTimerLabel
    {
        public static readonly string CheckPathValidity = nameof(CheckPathValidity);
        public static readonly string UpdateMoveRequest = nameof(UpdateMoveRequest);
        public static readonly string PathQueueUpdate = nameof(PathQueueUpdate);
        public static readonly string UpdateTopologyOptimization = nameof(UpdateTopologyOptimization);
        public static readonly string BuildProximityGrid = nameof(BuildProximityGrid);
        public static readonly string BuildNeighbours = nameof(BuildNeighbours);
        public static readonly string FindCorners = nameof(FindCorners);
        public static readonly string TriggerOffMeshConnections = nameof(TriggerOffMeshConnections);
        public static readonly string CalculateSteering = nameof(CalculateSteering);
        public static readonly string PlanVelocity = nameof(PlanVelocity);
        public static readonly string Integrate = nameof(Integrate);
        public static readonly string HandleCollisions = nameof(HandleCollisions);
        public static readonly string MoveAgents = nameof(MoveAgents);
        public static readonly string UpdateOffMeshConnections = nameof(UpdateOffMeshConnections);
    }
}