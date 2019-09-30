using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ISyntax
    {
        TextSpan Span { get; }
        string ToString();
    }
}
