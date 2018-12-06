using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// A non-missing token
    /// </summary>
    public interface IToken
    {
        TextSpan Span { get; }

        string Text(CodeText code);
    }
}
