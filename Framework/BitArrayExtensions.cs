using System;
using System.Collections;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class BitArrayExtensions
    {
        public static bool ValuesEqual(this BitArray left, BitArray right)
        {
            if (left.Count != right.Count) throw new InvalidOperationException();
            for (int i = 0; i < left.Count; i++)
                if (left[i] != right[i])
                    return false;

            return true;
        }

        public static IEnumerable<int> TrueIndexes(this BitArray array)
        {
            for (var i = 0; i < array.Length; i++)
                if (array[i])
                    yield return i;
        }

        public static IEnumerable<int> FalseIndexes(this BitArray array)
        {
            for (var i = 0; i < array.Length; i++)
                if (!array[i])
                    yield return i;
        }
    }
}
