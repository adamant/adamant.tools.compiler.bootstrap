using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ModifierParser : IParser<ModifierSyntax>
    {
        [CanBeNull] // This parser needs to be able to return null
        public ModifierSyntax Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case UnsafeKeywordToken unsafeKeyword:
                    tokens.MoveNext();
                    return new UnsafeModifierSyntax(unsafeKeyword);
                case AbstractKeywordToken abstractKeyword:
                    tokens.MoveNext();
                    return new AbstractModifierSyntax(abstractKeyword);
                case PublicKeywordToken _:
                case ProtectedKeywordToken _:
                case PrivateKeywordToken _:
                    return new AccessModifierSyntax(tokens.Expect<IAccessModifierToken>());
                default:
                    return null; // This parser needs to be able to return null
            }
        }
    }
}
