using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
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
            switch (Tokens.Current)
            {

                case IMutableKeywordToken _:
                {
                    var mutableKeyword = Tokens.Expect<IMutableKeywordToken>();
                    var referent = ParseType();
                    var span = TextSpan.Covering(mutableKeyword, referent.Span);
                    return new MutableTypeSyntax(span, referent);
                }
                case IIdentifierToken _:
                    return ParseTypeName();
                case IPrimitiveTypeToken _:
                    return ParsePrimitiveType();
                default:
                    throw new NotImplementedException();
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
