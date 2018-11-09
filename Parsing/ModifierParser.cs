using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
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
                case IImplicitKeywordToken implicitKeyword:
                    tokens.MoveNext();
                    return new ImplicitModiferSyntax(implicitKeyword);
                case IMoveKeywordToken moveKeyword:
                    tokens.MoveNext();
                    return new MoveModifierSyntax(moveKeyword);
                case IOverrideKeywordToken overrideKeyword:
                    tokens.MoveNext();
                    return new OverrideModifierSyntax(overrideKeyword);
                case IRefKeywordToken refKeyword:
                    tokens.MoveNext();
                    return new RefModifierSyntax(refKeyword);
                case IExtendKeywordToken extendKeyword:
                    tokens.MoveNext();
                    return new ExtendModifierSyntax(extendKeyword);
                case ISafeKeywordToken safeKeyword:
                    tokens.MoveNext();
                    return new SafeModifierSyntax(safeKeyword);
                case IUnsafeKeywordToken unsafeKeyword:
                    tokens.MoveNext();
                    return new UnsafeModifierSyntax(unsafeKeyword);
                case IAbstractKeywordToken abstractKeyword:
                    tokens.MoveNext();
                    return new AbstractModifierSyntax(abstractKeyword);
                case IMutableKeywordToken mutableKeyword:
                    tokens.MoveNext();
                    return new MutableModifierSyntax(mutableKeyword);
                case IPublicKeywordToken _:
                case IProtectedKeywordToken _:
                case IPrivateKeywordToken _:
                case IInternalKeywordToken _:
                    return new AccessModifierSyntax(tokens.Expect<IAccessModifierTokenPlace>());
                default:
                    return null; // This parser needs to be able to return null
            }
        }
    }
}
