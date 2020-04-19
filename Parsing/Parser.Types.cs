using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public ITypeSyntax ParseType()
        {
            var typeSyntax = ParseTypeWithReferenceCapability();

            IQuestionToken? question;
            while ((question = Tokens.AcceptToken<IQuestionToken>()) != null)
            {
                var span = TextSpan.Covering(typeSyntax.Span, question.Span);
                return new OptionalTypeSyntax(span, typeSyntax);
            }

            return typeSyntax;
        }

        private ITypeSyntax ParseTypeWithReferenceCapability()
        {
            switch (Tokens.Current)
            {
                case IOwnedKeywordToken _:
                {
                    var ownedKeyword = Tokens.RequiredToken<IOwnedKeywordToken>();
                    var mutableKeyword = Tokens.AcceptToken<IMutableKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(ownedKeyword.Span, referent.Span);
                    var capability = mutableKeyword == null
                        ? ReferenceCapability.Owned
                        : ReferenceCapability.OwnedMutable;
                    return new ReferenceCapabilityTypeSyntax(capability, referent, span);
                }
                case IMutableKeywordToken _:
                {
                    var mutableKeyword = Tokens.RequiredToken<IMutableKeywordToken>();
                    var referent = ParseBareType();
                    var span = TextSpan.Covering(mutableKeyword.Span, referent.Span);
                    return new MutableTypeSyntax(span, referent);
                }
                default:
                    return ParseBareType();
            }
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
                    name = SpecialName.Void;
                    break;
                case INeverKeywordToken _:
                    name = SpecialName.Never;
                    break;
                case IBoolKeywordToken _:
                    name = SpecialName.Bool;
                    break;
                case IAnyKeywordToken _:
                    name = SpecialName.Any;
                    break;
                case IByteKeywordToken _:
                    name = SpecialName.Byte;
                    break;
                case IIntKeywordToken _:
                    name = SpecialName.Int;
                    break;
                case IUIntKeywordToken _:
                    name = SpecialName.UInt;
                    break;
                case ISizeKeywordToken _:
                    name = SpecialName.Size;
                    break;
                case IOffsetKeywordToken _:
                    name = SpecialName.Offset;
                    break;
            }

            return new TypeNameSyntax(keyword.Span, name);
        }
    }
}
