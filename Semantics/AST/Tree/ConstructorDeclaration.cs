using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ConstructorDeclaration : InvocableDeclaration, IConstructorDeclaration
    {
        public IClassDeclaration DeclaringClass { get; }
        public new ConstructorSymbol Symbol { get; }
        public ISelfParameter ImplicitSelfParameter { get; }
        public IBody Body { get; }

        public ConstructorDeclaration(
            CodeFile file,
            TextSpan span,
            IClassDeclaration declaringClass,
            ConstructorSymbol symbol,
            TextSpan nameSpan,
            ISelfParameter implicitSelfParameter,
            FixedList<IConstructorParameter> parameters,
            IReachabilityAnnotations reachabilityAnnotations,
            IBody body)
            : base(file, span, symbol, nameSpan, parameters, reachabilityAnnotations)
        {
            Symbol = symbol;
            ImplicitSelfParameter = implicitSelfParameter;
            Body = body;
            DeclaringClass = declaringClass;
        }

        public override string ToString()
        {
            var name = Symbol.Name is null ? $" {Symbol.Name}" : "";
            return $"{Symbol.ContainingSymbol}::new{name}({string.Join(", ", Parameters)})";
        }
    }
}
