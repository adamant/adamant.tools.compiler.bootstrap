using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IStringLiteralToken : IToken
    {
        [NotNull] string Value { get; }
    }
}
