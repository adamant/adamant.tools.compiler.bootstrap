using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IConstructorDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        ISelfParameterSyntax ImplicitSelfParameter { get; }
        [DisallowNull] DataType? SelfParameterType { get; set; }
        new FixedList<IConstructorParameterSyntax> Parameters { get; }
    }
}
