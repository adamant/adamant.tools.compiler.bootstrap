using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Refs
{
    public struct Transition
    {
        public readonly Statement From;
        public readonly Statement To;

        public Transition(Statement from, Statement to)
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
