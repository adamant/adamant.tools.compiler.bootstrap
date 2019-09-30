using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, ICallableDeclarationSyntax
    {
        bool IsExternalFunction { get; set; }
        TypeSyntax? ReturnTypeSyntax { get; }
        new TypePromise ReturnType { get; }
    }
}
