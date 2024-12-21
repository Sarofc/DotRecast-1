using System;
using System.Buffers;

namespace DotRecast.Core
{
    /// <summary>
    /// <code>⚠️ use ArrayPool, need dispose</code>
    /// </summary>
    public ref struct RcObjImporterContext
    {
        private float[] _vertices;
        private int _nvertices;
        private int[] _faces;
        private int _nfaces;

        public ReadOnlySpan<float> Vertices => _vertices.AsSpan(0, _nvertices);
        public ReadOnlySpan<int> Faces => _faces.AsSpan(0, _nfaces);

        public RcObjImporterContext() : this(1024 * 16)
        { }

        public RcObjImporterContext(int capcatiy)
        {
            _vertices = ArrayPool<float>.Shared.Rent(capcatiy * 3);
            _faces = ArrayPool<int>.Shared.Rent(capcatiy);
        }

        public void AddVertex(float v)
        {
            if (_nvertices == _vertices.Length)
            {
                var newArray = ArrayPool<float>.Shared.Rent(_vertices.Length * 2);
                Array.Copy(_vertices, newArray, _nvertices);
                ArrayPool<float>.Shared.Return(_vertices);
                _vertices = newArray;
            }

            _vertices[_nvertices++] = v;
        }

        public void AddFace(int v)
        {
            if (_nfaces == _faces.Length)
            {
                var newArray = ArrayPool<int>.Shared.Rent(_faces.Length * 2);
                Array.Copy(_faces, newArray, _nfaces);
                ArrayPool<int>.Shared.Return(_faces);
                _faces = newArray;
            }

            _faces[_nfaces++] = v;
        }

        public void Dispose()
        {
            _nvertices = 0;
            _nfaces = 0;

            if (_vertices != null)
            {
                ArrayPool<float>.Shared.Return(_vertices);
                _vertices = null;
            }

            if (_faces != null)
            {
                ArrayPool<int>.Shared.Return(_faces);
                _faces = null;
            }
        }
    }
}