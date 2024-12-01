
using System;
using System.Runtime.CompilerServices;

namespace DotRecast.Core
{
    public struct Int3
    {
        public int X;
        public int Y;
        public int Z;

        public int this[int index]
        {
            readonly get
            {
                if ((uint)index >= 3) // Use unsigned comparison to eliminate bounds check
                {
                    ThrowIndexOutOfRangeException();
                }

                return Unsafe.Add(ref Unsafe.As<Int3, int>(ref Unsafe.AsRef(in this)), index);
            }
            set
            {
                if ((uint)index >= 3)
                {
                    ThrowIndexOutOfRangeException();
                }

                Unsafe.Add(ref Unsafe.As<Int3, int>(ref this), index) = value;
            }
        }

        private static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
    }
}
