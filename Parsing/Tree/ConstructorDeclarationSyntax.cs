using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ConstructorDeclarationSyntax : InvocableDeclarationSyntax, IConstructorDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public ISelfParameterSyntax ImplicitSelfParameter { get; }
        public new FixedList<IConstructorParameterSyntax> Parameters { get; }
        public virtual IBodySyntax Body { get; }
        public new Promise<ConstructorSymbol> Symbol { get; }

        public ConstructorDeclarationSyntax(
            IClassDeclarationSyntax declaringType,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name? name,
            ISelfParameterSyntax implicitSelfParameter,
            FixedList<IConstructorParameterSyntax> parameters,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, nameSpan, name, parameters, reachabilityAnnotations,
                new Promise<ConstructorSymbol>())
        {
            DeclaringClass = declaringType;
            ImplicitSelfParameter = implicitSelfParameter;
            Parameters = parameters;
            Body = body;
            Symbol = (Promise<ConstructorSymbol>)base.Symbol;
        }

        public override string ToString()
        {
            return Name is null
                ? $"new({string.Join(", ", Parameters)})"
                : $"new {Name}({string.Join(", ", Parameters)})";
        }
    }
}
