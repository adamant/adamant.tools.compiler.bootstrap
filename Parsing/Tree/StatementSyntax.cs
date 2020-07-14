using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;

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
