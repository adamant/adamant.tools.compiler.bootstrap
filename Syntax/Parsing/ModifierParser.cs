using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ModifierParser : IModifierParser
    {
        [CanBeNull]
        public ModifierSyntax AcceptModifier(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case ImplicitKeywordToken implicitKeyword:
                    tokens.MoveNext();
                    return new ImplicitModiferSyntax(implicitKeyword);
                case MoveKeywordToken moveKeyword:
                    tokens.MoveNext();
                    return new MoveModifierSyntax(moveKeyword);
                case OverrideKeywordToken overrideKeyword:
                    tokens.MoveNext();
                    return new OverrideModifierSyntax(overrideKeyword);
                case RefKeywordToken refKeyword:
                    tokens.MoveNext();
                    return new RefModifierSyntax(refKeyword);
                case ExtendKeywordToken extendKeyword:
                    tokens.MoveNext();
                    return new ExtendModifierSyntax(extendKeyword);
                case SafeKeywordToken safeKeyword:
                    tokens.MoveNext();
                    return new SafeModifierSyntax(safeKeyword);
                case UnsafeKeywordToken unsafeKeyword:
                    tokens.MoveNext();
                    return new UnsafeModifierSyntax(unsafeKeyword);
                case AbstractKeywordToken abstractKeyword:
                    tokens.MoveNext();
                    return new AbstractModifierSyntax(abstractKeyword);
                case MutableKeywordToken mutableKeyword:
                    tokens.MoveNext();
                    return new MutableModifierSyntax(mutableKeyword);
                case PublicKeywordToken _:
                case ProtectedKeywordToken _:
                case PrivateKeywordToken _:
                case InternalKeywordToken _:
                    return new AccessModifierSyntax(tokens.Expect<IAccessModifierToken>());
                default:
                    return null; // This parser needs to be able to return null
            }
        }
    }
}
