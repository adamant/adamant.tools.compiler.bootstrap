using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IL.Code;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Refs
{
    public class Edges : ReadOnlyCollection<Edge>
    {
        [NotNull] private readonly ILookup<BasicBlock, BasicBlock> from;
        [NotNull] private readonly ILookup<BasicBlock, BasicBlock> to;

        public Edges([NotNull] ILFunctionDeclaration function)
            : base(function.BasicBlocks.SelectMany(b => b.EndStatement.OutBlocks().Select(e => new Edge(b, function.BasicBlocks[e]))).ToList())
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
