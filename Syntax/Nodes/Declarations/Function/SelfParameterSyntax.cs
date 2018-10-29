using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Function
{
    public class SelfParameterSyntax : ParameterSyntax
    {
        [NotNull] public SelfKeywordToken SelfKeyword { get; }

        public SelfParameterSyntax([NotNull] SelfKeywordToken selfKeyword)
        {
            SelfKeyword = selfKeyword;
        }
    }
}
