using System;
using System.Buffers;
using DotRecast.Core;
using K4os.Compression.LZ4;

namespace DotRecast.Recast.Toolset;

public class LZ4Compressor : IRcCompressor
{
    public static readonly LZ4Compressor Shared = new();

    public byte[] Decompress(ReadOnlySpan<byte> input)
    {
        return LZ4Pickler.Unpickle(input);
    }

    public void Decompress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        LZ4Pickler.Unpickle(input, output);
    }

    public byte[] Compress(ReadOnlySpan<byte> input)
    {
        return LZ4Pickler.Pickle(input);
    }

    public void Compress<TBufferWriter>(ReadOnlySpan<byte> input, TBufferWriter outputWriter) where TBufferWriter : IBufferWriter<byte>
    {
        LZ4Pickler.Pickle(input, outputWriter);
    }
}