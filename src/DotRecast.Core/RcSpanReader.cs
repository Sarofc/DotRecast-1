using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace DotRecast.Core
{
    public ref struct RcSpanReader
    {
        private ReadOnlySpan<byte> _bytes;
        private int _position;

        public ReadOnlySpan<byte> UnreadSpan => _bytes.Slice(_position);

        public RcSpanReader(ReadOnlySpan<byte> bytes)
        {
            _bytes = bytes;
            _position = 0;
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

        ReadOnlySpan<byte> ReadBytes(int length)
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
            return BinaryPrimitives.ReadInt16LittleEndian(span);
        }


        public int ReadInt32()
        {
            var span = ReadBytes(4);
            return BinaryPrimitives.ReadInt32LittleEndian(span);
        }

        public float ReadSingle()
        {
            var span = ReadBytes(4);
            return BinaryPrimitives.ReadSingleLittleEndian(span);
        }

        public long ReadInt64()
        {
            var span = ReadBytes(8);
            return BinaryPrimitives.ReadInt64LittleEndian(span);
        }

    }

    public sealed class RcSpanWriter : IBufferWriter<byte> // lz4 不支持 ref struct 泛型，用class就行了
    {
        private byte[] _bytes;
        private int _position;

        public ReadOnlySpan<byte> WrittenSpan => _bytes.AsSpan(0, _position);

        public RcSpanWriter(byte[] bytes)
        {
            _bytes = bytes;
            _position = 0;
        }

        public unsafe void Write<T>(T value) where T : unmanaged
        {
            var bytes = GetSpan(sizeof(T));
            MemoryMarshal.Write(bytes, value);
            Advance(sizeof(T));
        }

        public unsafe void Write(ReadOnlySpan<byte> value)
        {
            var nbytes = value.Length;
            var bytes = GetSpan(nbytes);
            value.CopyTo(bytes);
            Advance(nbytes);
        }

        public void Advance(int count)
        {
            _position += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return _bytes.AsMemory(_position, sizeHint);
        }

        public Span<byte> GetSpan(int sizeHint)
        {
            return _bytes.AsSpan(_position, sizeHint);
        }
    }
}
