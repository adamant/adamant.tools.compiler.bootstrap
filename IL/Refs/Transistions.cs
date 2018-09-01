using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Refs
{
    public class Transistions : ReadOnlyCollection<Transistion>
    {
        private readonly ILookup<Statement, Statement> from;
        private readonly ILookup<Statement, Statement> to;

        public Transistions(FunctionDeclaration function)
            : base(function.BasicBlocks.SelectMany(b => b.Statements.Zip(b.Statements.Skip(1), (s1, s2) => new Transistion(s1, s2)))
                  .Concat(function.Edges().Select(e => new Transistion(e.From.EndStatement, e.To.Statements.First())))
                  .ToList())
        {
            from = this.ToLookup(e => e.From, e => e.To);
            to = this.ToLookup(e => e.To, e => e.From);
        }


        public IEnumerable<Statement> From(Statement statement)
        {
            return from[statement];
        }

        public IEnumerable<Statement> To(Statement statement)
        {
            return to[statement];
        }
    }
}
