using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull]
        public FixedList<UsingDirectiveSyntax> ParseUsingDirectives()
        {
            return AcceptList(AcceptUsingDirective);
        }

        [CanBeNull]
        public UsingDirectiveSyntax AcceptUsingDirective()
        {
            if (!Tokens.Accept<IUsingKeywordToken>()) return null;
            var name = ParseName();
            Tokens.Expect<ISemicolonToken>();
            return new UsingDirectiveSyntax(name);
        }
    }
}
