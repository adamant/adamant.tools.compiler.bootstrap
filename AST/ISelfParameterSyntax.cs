namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ISelfParameterSyntax : IParameterSyntax, IMethodParameterSyntax
    {
        bool MutableSelf { get; }
    }
}
