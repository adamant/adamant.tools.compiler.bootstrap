using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class MethodDeclarationSyntax : InvocableDeclarationSyntax, IMethodDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public new Name Name { get; }
        public ISelfParameterSyntax SelfParameter { get; }
        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnType { get; }
        public new AcyclicPromise<MethodSymbol> Symbol { get; }

        protected MethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name name,
            ISelfParameterSyntax selfParameter,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnType,
            IReachabilityAnnotationsSyntax reachabilityAnnotations)
            : base(span, file, accessModifier, nameSpan, name,
                parameters, reachabilityAnnotations, new AcyclicPromise<MethodSymbol>())
        {
            DeclaringClass = declaringClass;
            Name = name;
            SelfParameter = selfParameter;
            Parameters = parameters;
            ReturnType = returnType;
            Symbol = (AcyclicPromise<MethodSymbol>)base.Symbol;
        }
    }
}
