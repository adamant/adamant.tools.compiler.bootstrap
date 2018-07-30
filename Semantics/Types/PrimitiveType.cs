using System.ComponentModel;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class PrimitiveType : ObjectType
    {
        public static readonly PrimitiveType Void = new PrimitiveType(PrimitiveTypeName.Void);
        public static readonly PrimitiveType Never = new PrimitiveType(PrimitiveTypeName.Never);
        public static readonly PrimitiveType Int = new PrimitiveType(PrimitiveTypeName.Int);
        public static readonly PrimitiveType Bool = new PrimitiveType(PrimitiveTypeName.Bool);

        public PrimitiveTypeKind Kind { get; }

        private PrimitiveType(PrimitiveTypeName name)
          : base(name, false)
        {
            Kind = name.Kind;
        }

        public static PrimitiveType New(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.IntKeyword:
                    return Int;
                case TokenKind.VoidKeyword:
                    return Void;
                case TokenKind.BoolKeyword:
                    return Bool;
                default:
                    throw new InvalidEnumArgumentException($"Token kind `{kind}` is not a primitive type keyword");
            }
        }
    }
}
