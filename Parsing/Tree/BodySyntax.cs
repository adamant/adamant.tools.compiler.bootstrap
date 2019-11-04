using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BodySyntax : Syntax, IBodySyntax
    {
        public FixedList<IBodyStatementSyntax> Statements { get; }
        private readonly FixedList<IStatementSyntax> statements;
        FixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements => statements;

        public BodySyntax(TextSpan span, FixedList<IBodyStatementSyntax> statements)
            : base(span)
        {
            Statements = statements;
            this.statements = statements.ToFixedList<IStatementSyntax>();
        }

        public override string ToString()
        {
            return "{ … }";
        }
    }
}
