﻿using System;
using System.Collections.Generic;
using DotRecast.Core.Numerics;
using System.Numerics;

namespace DotRecast.Detour
{
    public class DtNavMeshQueryMock : DtNavMeshQuery
    {
        private readonly DtStraightPath[] _straightPath;
        private readonly DtStatus _status;

        public DtNavMeshQueryMock(DtStraightPath[] straightPath, DtStatus status)
            : base(null)
        {
            _straightPath = straightPath;
            _status = status;
        }

        public override DtStatus FindStraightPath(Vector3 startPos, Vector3 endPos,
            List<long> path, int pathSize,
            Span<DtStraightPath> straightPath, out int straightPathCount, int maxStraightPath,
            int options)
        {
            straightPathCount = 0;
            for (int i = 0; i < _straightPath.Length && i < maxStraightPath; ++i)
            {
                straightPath[i] = _straightPath[i];
                straightPathCount += 1;
            }

            return _status;
        }
    }
}