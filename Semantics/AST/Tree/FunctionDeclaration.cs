using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FunctionDeclaration : InvocableDeclaration, IFunctionDeclaration
    {
        public new FunctionSymbol Symbol { get; }
        public new FixedList<INamedParameter> Parameters { get; }

        public FunctionDeclaration(TextSpan span, FunctionSymbol symbol, FixedList<INamedParameter> parameters)
            : base(span, symbol, parameters.ToFixedList<IConstructorParameter>())
        {
            Symbol = symbol;
            Parameters = parameters;
        }

    }
}
