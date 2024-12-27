
using System;
using System.Runtime.CompilerServices;

namespace DotRecast.Core
{
    public struct UShort3
    {
        public ushort X;
        public ushort Y;
        public ushort Z;

        public ushort this[int index]
        {
            readonly get
            {
                if ((uint)index >= 3) // Use unsigned comparison to eliminate bounds check
                {
                    ThrowIndexOutOfRangeException();
                }

                return Unsafe.Add(ref Unsafe.As<UShort3, ushort>(ref Unsafe.AsRef(in this)), index);
            }
            set
            {
                if ((uint)index >= 3)
                {
                    ThrowIndexOutOfRangeException();
                }

                Unsafe.Add(ref Unsafe.As<UShort3, ushort>(ref this), index) = value;
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
