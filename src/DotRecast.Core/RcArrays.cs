using System;

namespace DotRecast.Core
{
    public static class RcArrays
    {
        // Type Safe Copy
        public static void Copy<T>(T[] sourceArray, int sourceIndex, T[] destinationArray, int destinationIndex, int length)
        {
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

        public static void Copy<T>(ReadOnlySpan<T> sourceArray, int sourceIndex, Span<T> destinationArray, int destinationIndex, int length)
        {
            sourceArray.Slice(sourceIndex, length).CopyTo(destinationArray.Slice(destinationIndex));
        }

        public static T[] CopyOf<T>(T[] source, int startIdx, int length)
        {
            var deatArr = new T[length];
            for (int i = 0; i < length; ++i)
            {
                deatArr[i] = source[startIdx + i];
            }
            return deatArr;
        }
    }
}