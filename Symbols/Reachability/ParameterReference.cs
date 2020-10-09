using System;
using System.Collections.Concurrent;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    public class ParameterReference : Reference
    {
        public int ParameterNumber { get; }
        private static readonly ConcurrentDictionary<int, ParameterReference> cache = new ConcurrentDictionary<int, ParameterReference>();

        public static ParameterReference Create(int parameterNumber)
        {
            return cache.GetOrAdd(parameterNumber, n => new ParameterReference(n));
        }

        private ParameterReference(int parameterNumber)
        {
            ParameterNumber = parameterNumber;
        }

        public override bool Equals(Reference? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ParameterReference reference
                && ParameterNumber == reference.ParameterNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParameterNumber);
        }
    }
}
