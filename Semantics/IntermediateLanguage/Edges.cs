using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class Edges : ReadOnlyCollection<Edge>
    {
        [NotNull] private readonly ILookup<BasicBlock, BasicBlock> from;
        [NotNull] private readonly ILookup<BasicBlock, BasicBlock> to;

        private Edges([NotNull] List<Edge> edges)
            : base(edges)
        {
            from = this.ToLookup(e => e.From, e => e.To);
            to = this.ToLookup(e => e.To, e => e.From);
        }

        [NotNull]
        public static Edges InGraph([NotNull] ControlFlowGraph controlFlowGraph)
        {
            Requires.NotNull(nameof(controlFlowGraph), controlFlowGraph);
            var edges = controlFlowGraph.BasicBlocks.SelectMany(b =>
                    b.Terminator.OutBlocks()
                        .Select(e => new Edge(b, controlFlowGraph.BasicBlocks[e])))
                .ToList();
            return new Edges(edges);
        }

        [NotNull, ItemNotNull]
        public IEnumerable<BasicBlock> From([NotNull] BasicBlock block)
        {
            return from[block];
        }

        [NotNull, ItemNotNull]
        public IEnumerable<BasicBlock> To([NotNull] BasicBlock block)
        {
            return to[block];
        }
    }
}
