using System.Collections;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public class FixedBitArray
    {
        private readonly BitArray values;

        public FixedBitArray(BitArray values)
        {
            this.values = new BitArray(values.Length);
            this.values.Or(values);
        }
    }
}
