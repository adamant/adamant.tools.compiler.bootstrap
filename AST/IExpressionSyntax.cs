using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IExpressionSyntax : ISyntax
    {
        DataType? Type { get; set; }
    }
}
