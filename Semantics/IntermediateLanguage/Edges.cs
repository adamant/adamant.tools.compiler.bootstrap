using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class Edges : ReadOnlyCollection<Edge>
    {
        [NotNull] private readonly ILookup<BasicBlock, BasicBlock> from;
        [NotNull] private readonly ILookup<BasicBlock, BasicBlock> to;

        public Edges([NotNull] ControlFlowGraph controlFlowGraph)
            : base(controlFlowGraph.BasicBlocks.SelectMany(b => b.EndStatement.OutBlocks().Select(e => new Edge(b, controlFlowGraph.BasicBlocks[e]))).ToList())
        {
            from = this.ToLookup(e => e.From, e => e.To);
            to = this.ToLookup(e => e.To, e => e.From);
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<BasicBlock> From([NotNull] BasicBlock block)
        {
            return from[block];
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<BasicBlock> To([NotNull] BasicBlock block)
        {
            return to[block];
        }
    }
}
