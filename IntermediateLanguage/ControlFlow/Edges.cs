using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Edges : ReadOnlyCollection<Edge>
    {
        private readonly ILookup<BasicBlock, BasicBlock> from;
        private readonly ILookup<BasicBlock, BasicBlock> to;

        private Edges(List<Edge> edges)
            : base(edges)
        {
            from = this.ToLookup(e => e.From, e => e.To);
            to = this.ToLookup(e => e.To, e => e.From);
        }

        internal static Edges InGraph(ControlFlowGraph controlFlowGraph)
        {
            var edges = controlFlowGraph.BasicBlocks.SelectMany(b =>
                    b.Terminator.OutBlocks()
                        .Select(e => new Edge(b, controlFlowGraph.BasicBlocks[e])))
                .ToList();
            return new Edges(edges);
        }

        public IEnumerable<BasicBlock> From(BasicBlock block)
        {
            return from[block];
        }

        public IEnumerable<BasicBlock> To(BasicBlock block)
        {
            return to[block];
        }
    }
}
