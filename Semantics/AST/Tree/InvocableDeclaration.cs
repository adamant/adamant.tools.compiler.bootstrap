using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class InvocableDeclaration : Declaration, IInvocableDeclaration
    {
        public new InvocableSymbol Symbol { get; }
        public FixedList<IConstructorParameter> Parameters { get; }

        protected InvocableDeclaration(
            CodeFile file,
            TextSpan span,
            InvocableSymbol symbol,
            FixedList<IConstructorParameter> parameters)
            : base(file, span, symbol)
        {
            Symbol = symbol;
            Parameters = parameters;
        }
    }
}
