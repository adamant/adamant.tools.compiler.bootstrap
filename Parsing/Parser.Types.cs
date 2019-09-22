using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public TypeSyntax ParseType()
        {
            switch (Tokens.Current)
            {
                case ISelfTypeKeywordToken _:
                    var selfTypeKeyword = Tokens.Expect<ISelfTypeKeywordToken>();
                    return new SelfTypeSyntax(selfTypeKeyword);
                //case IDollarToken _:
                //    if (minPrecedence <= OperatorPrecedence.Lifetime)
                //    {
                //        Tokens.RequiredToken<IDollarToken>();
                //        var (nameSpan, lifetime) = ParseLifetimeName();
                //        expression = new ReferenceLifetimeSyntax(expression, nameSpan, lifetime);
                //        continue;
                //    }

                //    break;
                case IIdentifierToken _:
                    return ParseTypeName();
                case IPrimitiveTypeToken _:
                    return ParsePrimitiveType();
                default:
                    throw new NotImplementedException();
            }
        }

        private TypeNameSyntax ParseTypeName()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = new SimpleName(identifier.Value);
            return new TypeNameSyntax(identifier.Span, name);
        }

        private TypeNameSyntax ParsePrimitiveType()
        {
            var keyword = Tokens.RequiredToken<IPrimitiveTypeToken>();
            SimpleName name;
            switch (keyword)
            {
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
                default:
                    throw NonExhaustiveMatchException.For(keyword);
            }

            return new TypeNameSyntax(keyword.Span, name);
        }
    }
}
