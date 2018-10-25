using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts
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
