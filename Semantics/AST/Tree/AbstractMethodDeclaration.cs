using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class AbstractMethodDeclaration : InvocableDeclaration, IAbstractMethodDeclaration
    {
        public new MethodSymbol Symbol { get; }
        public ISelfParameter SelfParameter { get; }
        public new FixedList<INamedParameter> Parameters { get; }

        public AbstractMethodDeclaration(
            CodeFile file,
            TextSpan span,
            MethodSymbol symbol,
            ISelfParameter selfParameter,
            FixedList<INamedParameter> parameters)
            : base(file, span, symbol, parameters.ToFixedList<IConstructorParameter>())
        {
            Symbol = symbol;
            SelfParameter = selfParameter;
            Parameters = parameters;
        }
    }
}
