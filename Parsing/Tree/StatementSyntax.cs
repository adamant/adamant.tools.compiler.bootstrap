using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{

    internal abstract class StatementSyntax : Syntax, IStatementSyntax
    {
        private protected StatementSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
