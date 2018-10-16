using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Graph;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Refs
{
    public struct Edge
    {
        public readonly BasicBlock From;
        public readonly BasicBlock To;

        public Edge(BasicBlock from, BasicBlock to)
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
