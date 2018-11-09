using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class MemberDeclarationSyntax : DeclarationSyntax
    {
        public TextSpan SignatureSpan { get; }

        protected MemberDeclarationSyntax(TextSpan signatureSpan)
        {
            SignatureSpan = signatureSpan;
        }
    }
}
