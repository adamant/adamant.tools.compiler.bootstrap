using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AssociatedFunctionDeclarationSyntax : InvocableDeclarationSyntax, IAssociatedFunctionDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public new Name Name { get; }
        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnType { get; }
        public IBodySyntax Body { get; }
        public new Promise<FunctionSymbol> Symbol { get; }

        public AssociatedFunctionDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name name,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, fullName, nameSpan, name, parameters,
                reachabilityAnnotations, new Promise<FunctionSymbol>())
        {
            DeclaringClass = declaringClass;
            Name = name;
            Parameters = parameters;
            ReturnType = returnTypeSyntax;
            Body = body;
            Symbol = (Promise<FunctionSymbol>)base.Symbol;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType} {Body}";
        }
    }
}
