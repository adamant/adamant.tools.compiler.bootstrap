using System;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
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
