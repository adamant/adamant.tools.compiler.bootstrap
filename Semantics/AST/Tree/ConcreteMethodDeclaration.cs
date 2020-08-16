using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ConcreteMethodDeclaration : InvocableDeclaration, IConcreteMethodDeclaration
    {
        public new MethodSymbol Symbol { get; }
        public ISelfParameter SelfParameter { get; }
        public new FixedList<INamedParameter> Parameters { get; }
        public IBody Body { get; }

        public ConcreteMethodDeclaration(
            CodeFile file,
            TextSpan span,
            MethodSymbol symbol,
            ISelfParameter selfParameter,
            FixedList<INamedParameter> parameters,
            IBody body)
            : base(file, span, symbol, parameters.ToFixedList<IConstructorParameter>())
        {
            Symbol = symbol;
            Parameters = parameters;
            SelfParameter = selfParameter;
            Body = body;
        }
    }
}
