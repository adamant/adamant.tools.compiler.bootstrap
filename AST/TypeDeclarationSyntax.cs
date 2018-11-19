using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeDeclarationSyntax : MemberDeclarationSyntax
    {
        protected TypeDeclarationSyntax(TextSpan nameSpan)
            : base(nameSpan)
        {
        }
    }
}
