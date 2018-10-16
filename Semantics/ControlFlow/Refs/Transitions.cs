using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Refs
{
    public class Transitions : ReadOnlyCollection<Transition>
    {
        [NotNull] private readonly ILookup<Statement, Statement> from;
        [NotNull] private readonly ILookup<Statement, Statement> to;

        public Transitions([NotNull] ControlFlowGraph controlFlowGraph)
            : base(controlFlowGraph.BasicBlocks.SelectMany(b => b.Statements.Zip(b.Statements.Skip(1), (s1, s2) => new Transition(s1, s2)))
                  .Concat(controlFlowGraph.Edges().Select(e => new Transition(e.From.EndStatement, e.To.Statements.First())))
                  .ToList())
        {
            from = this.ToLookup(e => e.From, e => e.To);
            to = this.ToLookup(e => e.To, e => e.From);
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Statement> From(Statement statement)
        {
            return from[statement];
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Statement> To(Statement statement)
        {
            return to[statement];
        }
    }
}
