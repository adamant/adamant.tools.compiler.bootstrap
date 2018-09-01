using System;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Refs
{
    public struct Transistion
    {
        public readonly Statement From;
        public readonly Statement To;

        public Transistion(Statement from, Statement to)
        {
            From = from;
            To = to;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }
    }
}
