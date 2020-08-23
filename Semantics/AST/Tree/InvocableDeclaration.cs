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
        public IReachabilityAnnotations ReachabilityAnnotations { get; }

        protected InvocableDeclaration(
            CodeFile file,
            TextSpan span,
            InvocableSymbol symbol,
            TextSpan nameSpan,
            FixedList<IConstructorParameter> parameters,
            IReachabilityAnnotations reachabilityAnnotations)
            : base(file, span, symbol, nameSpan)
        {
            Symbol = symbol;
            Parameters = parameters;
            ReachabilityAnnotations = reachabilityAnnotations;
        }
    }
}
