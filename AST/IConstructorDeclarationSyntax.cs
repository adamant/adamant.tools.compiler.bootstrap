using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        [DisallowNull] DataType? SelfParameterType { get; set; }
    }
}
