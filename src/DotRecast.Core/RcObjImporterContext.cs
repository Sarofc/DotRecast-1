using System;
using System.Buffers;
using System.Collections.Generic;

namespace DotRecast.Core
{
    public class RcObjImporterContext
    {
        public List<float> Vertices { get; private set; }
        public List<int> Faces { get; private set; }

        public RcObjImporterContext() : this(1024 * 16)
        { }

        public RcObjImporterContext(int capcatiy)
        {
            Vertices = new(capcatiy * 3);
            Faces = new(capcatiy);
        }
    }
}