using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class InvocableDeclaration : Declaration, IInvocableDeclaration
    {
        public FixedList<IConstructorParameter> Parameters { get; }

        protected InvocableDeclaration(TextSpan span, Symbol symbol, FixedList<IConstructorParameter> parameters)
            : base(span, symbol)
        {
            Parameters = parameters;
        }
    }
}
