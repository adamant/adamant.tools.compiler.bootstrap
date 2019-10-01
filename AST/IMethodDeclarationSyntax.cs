using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        new FixedList<IParameterSyntax> Parameters { get; }
        [DisallowNull] DataType? SelfParameterType { get; set; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
