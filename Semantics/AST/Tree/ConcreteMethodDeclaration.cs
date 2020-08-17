using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using MoreLinq.Extensions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ConcreteMethodDeclaration : InvocableDeclaration, IConcreteMethodDeclaration
    {
        public IClassDeclaration DeclaringClass { get; }
        public new MethodSymbol Symbol { get; }
        public ISelfParameter SelfParameter { get; }
        public new FixedList<INamedParameter> Parameters { get; }
        public IBody Body { get; }

        public ConcreteMethodDeclaration(
            CodeFile file,
            TextSpan span,
            IClassDeclaration declaringClass,
            MethodSymbol symbol,
            TextSpan nameSpan,
            ISelfParameter selfParameter,
            FixedList<INamedParameter> parameters,
            IBody body)
            : base(file, span, symbol, nameSpan, parameters.ToFixedList<IConstructorParameter>())
        {
            Symbol = symbol;
            Parameters = parameters;
            SelfParameter = selfParameter;
            Body = body;
            DeclaringClass = declaringClass;
        }

        public override string ToString()
        {
            var returnType = Symbol.ReturnDataType != null ? " -> " + Symbol.ReturnDataType : "";
            return $"fn {Symbol.ContainingSymbol}::{Symbol.Name}({string.Join(", ", Parameters.Prepend<IParameter>(SelfParameter))}){returnType} {Body}";
        }
    }
}
