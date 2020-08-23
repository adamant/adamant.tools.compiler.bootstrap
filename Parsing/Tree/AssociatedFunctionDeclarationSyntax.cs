using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
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
        public new AcyclicPromise<FunctionSymbol> Symbol { get; }

        public AssociatedFunctionDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name name,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            IReachabilityAnnotationsSyntax reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, nameSpan, name, parameters,
                reachabilityAnnotations, new AcyclicPromise<FunctionSymbol>())
        {
            DeclaringClass = declaringClass;
            Name = name;
            Parameters = parameters;
            ReturnType = returnTypeSyntax;
            Body = body;
            Symbol = (AcyclicPromise<FunctionSymbol>)base.Symbol;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {Name}({string.Join(", ", Parameters)}){returnType} {Body}";
        }
    }
}
