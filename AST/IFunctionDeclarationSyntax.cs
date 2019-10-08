using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteCallableDeclarationSyntax
    {
        bool IsExternalFunction { get; }
        ITypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
