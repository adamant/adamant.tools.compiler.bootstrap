using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public ITypeSyntax ParseType()
        {
            var typeSyntax = ParseTypeWithCapability();

            IQuestionToken? question;
            while ((question = Tokens.AcceptToken<IQuestionToken>()) != null)
            {
                var span = TextSpan.Covering(typeSyntax.Span, question.Span);
                return new OptionalTypeSyntax(span, typeSyntax);
            }

            return typeSyntax;
        }

        private ITypeSyntax ParseTypeWithCapability()
        {
            switch (Tokens.Current)
            {
                case IOwnedKeywordToken _:
                    return ParseTypeWithCapability<IOwnedKeywordToken>(Owned, OwnedMutable);
                case IIsolatedKeywordToken _:
                    return ParseTypeWithCapability<IIsolatedKeywordToken>(Isolated, IsolatedMutable);
                case IHeldKeywordToken _:
                    return ParseTypeWithCapability<IHeldKeywordToken>(Held, HeldMutable);
                case IMutableKeywordToken _:
                {
                    var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                    return ParseMutableType(mutableKeyword);
                }
                case IIdKeywordToken _:
                {
                    var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(mutableKeyword.Span, referent.Span);
                    return new CapabilityTypeSyntax(Identity, referent, span);
                }
                default:
                    return ParseBareType();
            }
        }

        /// <summary>
        /// Parse the type after a single `mut` keyword. Requires that the mutable
        /// keyword has already been consumed.
        /// </summary>
        private ITypeSyntax ParseMutableType(IMutableKeywordToken mutableKeyword)
        {
            var referent = ParseBareType();
            var span = TextSpan.Covering(mutableKeyword.Span, referent.Span);
            return new CapabilityTypeSyntax(Borrowed, referent, span);
        }

        private ITypeSyntax ParseTypeWithCapability<TCapabilityToken>(ReferenceCapability immutableCapability, ReferenceCapability mutableCapability)
            where TCapabilityToken : ICapabilityToken
        {
            var primaryCapability = Tokens.RequiredToken<TCapabilityToken>();
            var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
            var referent = ParseBareType();
            var span = TextSpan.Covering(primaryCapability.Span, referent.Span);
            var capability = mutableKeyword is null ? immutableCapability : mutableCapability;
            return new CapabilityTypeSyntax(capability, referent, span);
        }

        private ITypeSyntax ParseBareType()
        {
            switch (Tokens.Current)
            {
                case IPrimitiveTypeToken _:
                    return ParsePrimitiveType();
                default: // otherwise we want a type name
                    return ParseTypeName();
            }
        }

        private ITypeNameSyntax ParseTypeName()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = new SimpleName(identifier.Value);
            return new TypeNameSyntax(identifier.Span, name);
        }

        private ITypeNameSyntax ParsePrimitiveType()
        {
            var keyword = Tokens.RequiredToken<IPrimitiveTypeToken>();
            SimpleName name;
            switch (keyword)
            {
                default:
                    throw ExhaustiveMatch.Failed(keyword);
                case IVoidKeywordToken _:
                    name = SpecialNames.Void;
                    break;
                case INeverKeywordToken _:
                    name = SpecialNames.Never;
                    break;
                case IBoolKeywordToken _:
                    name = SpecialNames.Bool;
                    break;
                case IAnyKeywordToken _:
                    name = SpecialNames.Any;
                    break;
                case IByteKeywordToken _:
                    name = SpecialNames.Byte;
                    break;
                case IIntKeywordToken _:
                    name = SpecialNames.Int;
                    break;
                case IUIntKeywordToken _:
                    name = SpecialNames.UInt;
                    break;
                case ISizeKeywordToken _:
                    name = SpecialNames.Size;
                    break;
                case IOffsetKeywordToken _:
                    name = SpecialNames.Offset;
                    break;
            }

            return new TypeNameSyntax(keyword.Span, name);
        }
    }
}
