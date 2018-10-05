using System;
using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public static class TokenKindExtensions
    {
        public static bool IsOperator(this TokenKind tokenKind)
        {
            return tokenKind >= TokenKind.FirstOperator && tokenKind <= TokenKind.LastOperator;
        }

        public static bool IsIdentifier(this TokenKind tokenKind)
        {
            return tokenKind == TokenKind.Identifier
                   || tokenKind == TokenKind.EscapedIdentifier
                   || tokenKind == TokenKind.EscapedStringIdentifier;
        }

        public static bool HasValue(this TokenKind tokenKind)
        {
            switch (tokenKind)
            {
                case TokenKind.Identifier:
                case TokenKind.EscapedIdentifier:
                case TokenKind.EscapedStringIdentifier:
                case TokenKind.StringLiteral:
                case TokenKind.IntegerLiteral:
                    return true;
                default:
                    return false;
            }
        }

        public static Type ValueType(this TokenKind tokenKind)
        {
            switch (tokenKind)
            {
                case TokenKind.Identifier:
                case TokenKind.EscapedIdentifier:
                case TokenKind.EscapedStringIdentifier:
                case TokenKind.StringLiteral:
                    return typeof(string);
                case TokenKind.IntegerLiteral:
                    return typeof(BigInteger);
                default:
                    return typeof(void);
            }
        }
    }
}
