using System.ComponentModel;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class PrimitiveType : ObjectType
    {
        public static readonly PrimitiveType Int = new PrimitiveType(TokenKind.IntKeyword);
        public static readonly PrimitiveType Void = new PrimitiveType(TokenKind.VoidKeyword);

        public TokenKind Kind { get; }

        private PrimitiveType(TokenKind kind)
        {
            Kind = kind;
        }

        public static PrimitiveType New(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.IntKeyword:
                    return Int;
                case TokenKind.VoidKeyword:
                    return Void;
                default:
                    throw new InvalidEnumArgumentException($"Token kind `{kind}` is not a primitive type keyword");
            }
        }
    }
}
