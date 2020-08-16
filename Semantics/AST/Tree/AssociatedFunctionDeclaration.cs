using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class AssociatedFunctionDeclaration : InvocableDeclaration, IAssociatedFunctionDeclaration
    {
        public new FunctionSymbol Symbol { get; }
        public new FixedList<INamedParameter> Parameters { get; }
        public IBody Body { get; }

        public AssociatedFunctionDeclaration(
            CodeFile file,
            TextSpan span,
            FunctionSymbol symbol,
            FixedList<INamedParameter> parameters,
            IBody body)
            : base(file, span, symbol, parameters.ToFixedList<IConstructorParameter>())
        {
            Symbol = symbol;
            Parameters = parameters;
            Body = body;
        }
    }
}
