using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    // TODO No error is reported if IConcreteMethodDeclarationSyntax is missing
    internal class ConcreteMethodDeclarationSyntax : MethodDeclarationSyntax, IConcreteMethodDeclarationSyntax
    {
        public virtual FixedList<IStatementSyntax> Body { get; }

        public ConcreteMethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<IParameterSyntax> parameters,
            IExpressionSyntax? lifetimeBounds,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IStatementSyntax> body)
            : base(declaringClass, span, file, modifiers, fullName, nameSpan, parameters, lifetimeBounds, returnTypeSyntax,
                ConcreteCallableDeclarationSyntax.GetChildSymbols(parameters, body))
        {
            Body = body;
        }

        public override string ToString()
        {
            var returnType = ReturnTypeSyntax != null ? " -> " + ReturnTypeSyntax : "";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType} {{ … }}";
        }
    }
}
