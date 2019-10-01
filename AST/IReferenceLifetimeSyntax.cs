using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IReferenceLifetimeSyntax : ITypeSyntax
    {
        ITypeSyntax ReferentType { get; }
        SimpleName Lifetime { get; }
    }
}
