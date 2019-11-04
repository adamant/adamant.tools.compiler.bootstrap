using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IAbstractMethodDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax))]
    public interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax
    {
        [DisallowNull] DataType? SelfParameterType { get; set; }
        // TODO since the parser requires the first param to be self, take it out of the parameter list
        new FixedList<IMethodParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
