using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        bool IsExternalFunction { get; }
        new FixedList<INamedParameterSyntax> Parameters { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
