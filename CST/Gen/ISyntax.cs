using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.CST.Gen
{
    public interface ISyntax
    {
        TextSpan Span { get; }
        string ToString();
    }
}
