using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public struct Optional<T>
        where T : Token
    {
        private readonly object value;

        public Optional(T value)
        {
            this.value = value;
        }

        public Optional(MissingToken value)
        {
            this.value = value;
        }

        public TextSpan Span
        {
            get
            {
                switch (value)
                {
                    case T token:
                        return token.Span;
                    case MissingToken missing:
                        return missing.Span;
                    default:
                        throw NonExhaustiveMatchException.For(value);
                }
            }
        }

        public Token ToNullableToken()
        {
            switch (value)
            {
                case T token:
                    return token;
                case MissingToken _:
                    return null;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static implicit operator Optional<T>(MissingToken value)
        {
            return new Optional<T>(value);
        }
    }
}
