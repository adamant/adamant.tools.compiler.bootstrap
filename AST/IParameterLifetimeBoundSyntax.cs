using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IParameterLifetimeBoundSyntax : ILifetimeBoundSyntax
    {
        SimpleName ParameterName { get; }
    }
}
