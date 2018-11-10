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
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case IImplicitKeywordToken implicitKeyword:
                    tokens.Next();
                    return new ImplicitModiferSyntax(implicitKeyword);
                case IMoveKeywordToken moveKeyword:
                    tokens.Next();
                    return new MoveModifierSyntax(moveKeyword);
                case IOverrideKeywordToken overrideKeyword:
                    tokens.Next();
                    return new OverrideModifierSyntax(overrideKeyword);
                case IRefKeywordToken refKeyword:
                    tokens.Next();
                    return new RefModifierSyntax(refKeyword);
                case IExtendKeywordToken extendKeyword:
                    tokens.Next();
                    return new ExtendModifierSyntax(extendKeyword);
                case ISafeKeywordToken safeKeyword:
                    tokens.Next();
                    return new SafeModifierSyntax(safeKeyword);
                case IUnsafeKeywordToken unsafeKeyword:
                    tokens.Next();
                    return new UnsafeModifierSyntax(unsafeKeyword);
                case IAbstractKeywordToken abstractKeyword:
                    tokens.Next();
                    return new AbstractModifierSyntax(abstractKeyword);
                case IMutableKeywordToken mutableKeyword:
                    tokens.Next();
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
