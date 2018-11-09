using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IStringLiteralToken : IToken
    {
        [NotNull] string Value { get; }
    }
}
