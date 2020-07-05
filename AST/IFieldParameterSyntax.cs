using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFieldParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        void SetIsMutableBinding(bool value);
        SimpleName FieldName { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
