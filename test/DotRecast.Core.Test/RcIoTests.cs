using System.IO;
using NUnit.Framework;

namespace DotRecast.Core.Test;

public class RcIoTests
{
    [Test]
    public void Test()
    {
        const long tileRef = 281474976710656L;
        const int dataSize = 344;

        byte[] actual;

        {
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);

            RcIO.Write(bw, tileRef);
            RcIO.Write(bw, dataSize);

            bw.Flush();
            actual = ms.ToArray();
        }

        {
            using MemoryStream ms = new(actual);
            using BinaryReader br = new(ms);
            var byteBuffer = RcIO.ToByteBuffer(br);

            Assert.That(byteBuffer.ReadInt64(), Is.EqualTo(tileRef));
            Assert.That(byteBuffer.ReadInt32(), Is.EqualTo(dataSize));
        }
    }
}