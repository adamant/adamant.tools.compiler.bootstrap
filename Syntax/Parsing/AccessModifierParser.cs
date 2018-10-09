using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class AccessModifierParser : IParser<AccessModifierSyntax>
    {
        [NotNull]
        public AccessModifierSyntax Parse([NotNull] ITokenStream tokens)
        {
            switch (tokens.Current)
            {
                case PublicKeywordToken _:
                case ProtectedKeywordToken _:
                case PrivateKeywordToken _:
                    return new AccessModifierSyntax(tokens.ExpectKeyword());
                default:
                    return new AccessModifierSyntax(tokens.MissingToken<KeywordToken>());
            }
        }
    }
}
