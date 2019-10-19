using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// A non-missing token
    /// </summary>
    [Closed(
        typeof(IEssentialToken),
        typeof(ITriviaToken))]
    public partial interface IToken
    {
        TextSpan Span { get; }

        string Text(CodeText code);
    }
}
