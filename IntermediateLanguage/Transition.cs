using System;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public struct Transition
    {
        public readonly ExpressionStatement From;
        public readonly ExpressionStatement To;

        public Transition(ExpressionStatement from, ExpressionStatement to)
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
