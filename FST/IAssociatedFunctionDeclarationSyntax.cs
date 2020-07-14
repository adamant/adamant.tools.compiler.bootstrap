using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
