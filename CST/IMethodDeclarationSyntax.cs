using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IAbstractMethodDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax))]
    public interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, ICallableDeclarationSyntax, IMethodMetadata
    {
        ISelfParameterSyntax SelfParameter { get; }
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new DataTypePromise ReturnDataType { get; }
    }
}
