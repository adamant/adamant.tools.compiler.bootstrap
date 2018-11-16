using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IIdentifierToken : IToken, IIdentifierOrPrimitiveToken
    {
        [NotNull] string Value { get; }
    }
}
