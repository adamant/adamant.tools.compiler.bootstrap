using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFieldParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        SimpleName FieldName { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
