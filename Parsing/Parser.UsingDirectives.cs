using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
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
            return AcceptMany(AcceptUsingDirective);
        }

        [CanBeNull]
        public UsingDirectiveSyntax AcceptUsingDirective()
        {
            if (!Tokens.Accept<IUsingKeywordToken>()) return null;
            var identifiers = AcceptOneOrMore<IIdentifierToken, IDotToken>(
                () => Tokens.AcceptToken<IIdentifierToken>());
            RootName name = GlobalNamespaceName.Instance;
            foreach (var identifier in identifiers)
                name = name.Qualify(identifier.Value);
            Tokens.Expect<ISemicolonToken>();
            return new UsingDirectiveSyntax((Name)name);
        }
    }
}
