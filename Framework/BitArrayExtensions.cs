using System;
using System.Collections;

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
    }
}
