using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IFieldParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
    {
        void SetIsMutableBinding(bool value);
        SimpleName FieldName { get; }
        IExpressionSyntax? DefaultValue { get; }
    }
}
