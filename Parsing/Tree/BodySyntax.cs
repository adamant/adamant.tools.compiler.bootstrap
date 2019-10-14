using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BodySyntax : Syntax, IBodySyntax
    {
        public FixedList<IStatementSyntax> Statements { get; }
        public BodySyntax(TextSpan span, FixedList<IStatementSyntax> statements)
            : base(span)
        {
            Statements = statements;
        }

        public override string ToString()
        {
            return "{ … }";
        }
    }
}
