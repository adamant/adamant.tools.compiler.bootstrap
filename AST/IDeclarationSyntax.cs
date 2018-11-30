using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IDeclarationSyntax
    {
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DeclarationSyntax AsDeclarationSyntax { get; }
    }
}
