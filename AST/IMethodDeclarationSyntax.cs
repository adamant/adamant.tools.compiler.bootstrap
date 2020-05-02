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
        ISelfParameterSyntax SelfParameter { get; }
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
