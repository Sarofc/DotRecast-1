using System;
using System.Buffers.Binary;

namespace DotRecast.Core
{
    public ref struct RcByteBuffer
    {
        private RcByteOrder _order;
        private Span<byte> _bytes;
        private int _position;

        public RcByteBuffer(Span<byte> bytes)
        {
            _order = BitConverter.IsLittleEndian
                ? RcByteOrder.LITTLE_ENDIAN
                : RcByteOrder.BIG_ENDIAN;

            _bytes = bytes;
            _position = 0;
        }

        public RcByteOrder Order()
        {
            return _order;
        }

        public void Order(RcByteOrder order)
        {
            _order = order;
        }

        public int Limit()
        {
            return _bytes.Length - _position;
        }

        public int Remaining()
        {
            int rem = Limit();
            return rem > 0 ? rem : 0;
        }


        public void Position(int pos)
        {
            _position = pos;
        }

        public int Position()
        {
            return _position;
        }

        public Span<byte> ReadBytes(int length)
        {
            var nextPos = _position + length;
            (nextPos, _position) = (_position, nextPos);

            return _bytes.Slice(nextPos, length);
        }

        public byte ReadByte()
        {
            var span = ReadBytes(1);
            return span[0];
        }

        public short ReadInt16()
        {
            var span = ReadBytes(2);
            if (_order == RcByteOrder.BIG_ENDIAN)
            {
                return BinaryPrimitives.ReadInt16BigEndian(span);
            }
            else
            {
                return BinaryPrimitives.ReadInt16LittleEndian(span);
            }
        }


        public int ReadInt32()
        {
            var span = ReadBytes(4);
            if (_order == RcByteOrder.BIG_ENDIAN)
            {
                return BinaryPrimitives.ReadInt32BigEndian(span);
            }
            else
            {
                return BinaryPrimitives.ReadInt32LittleEndian(span);
            }
        }

        public float ReadSingle()
        {
            var span = ReadBytes(4);
            if (_order == RcByteOrder.BIG_ENDIAN && BitConverter.IsLittleEndian)
            {
                span.Reverse();
            }
            else if (_order == RcByteOrder.LITTLE_ENDIAN && !BitConverter.IsLittleEndian)
            {
                span.Reverse();
            }

            return BitConverter.ToSingle(span);
        }

        public long ReadInt64()
        {
            var span = ReadBytes(8);
            if (_order == RcByteOrder.BIG_ENDIAN)
            {
                return BinaryPrimitives.ReadInt64BigEndian(span);
            }
            else
            {
                return BinaryPrimitives.ReadInt64LittleEndian(span);
            }
        }
    }
}
