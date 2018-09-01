using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IL.Code;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Refs
{
    public class Edges : ReadOnlyCollection<Edge>
    {
        private readonly ILookup<BasicBlock, BasicBlock> from;
        private readonly ILookup<BasicBlock, BasicBlock> to;

        public Edges(FunctionDeclaration function)
            : base(function.BasicBlocks.SelectMany(b => b.EndStatement.OutBlocks().Select(e => new Edge(b, function.BasicBlocks[e]))).ToList())
        {
            from = this.ToLookup(e => e.From, e => e.To);
            to = this.ToLookup(e => e.To, e => e.From);
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
