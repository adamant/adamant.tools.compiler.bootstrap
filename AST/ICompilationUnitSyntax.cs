using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface ICompilationUnitSyntax : ISyntax
    {
        CodeFile CodeFile { get; }
        RootName ImplicitNamespaceName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<INonMemberDeclarationSyntax> Declarations { get; }
        FixedList<IEntityDeclarationSyntax> EntityDeclarations { get; }
        FixedList<Diagnostic> Diagnostics { get; }
    }
}
