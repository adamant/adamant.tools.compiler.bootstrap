using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        public bool IsExternalFunction { get; set; }
        public ExpressionSyntax LifetimeBounds { get; }
        public TypeSyntax ReturnTypeSyntax { get; }

        public NamedFunctionDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<ParameterSyntax> parameters, // For now we will not support pure meta functions
            ExpressionSyntax lifetimeBounds,
            TypeSyntax returnTypeSyntax,
            BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, parameters, body)
        {
            LifetimeBounds = lifetimeBounds;
            ReturnTypeSyntax = returnTypeSyntax;
        }

        public override string ToString()
        {
            var returnType = ReturnTypeSyntax != null ? " -> " + ReturnTypeSyntax : "";
            var body = Body != null ? " {{ … }}" : ";";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType}{body}";
        }
    }
}
