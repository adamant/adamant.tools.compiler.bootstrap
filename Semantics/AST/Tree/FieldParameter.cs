using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class FieldParameter : Parameter, IFieldParameter
    {
        public FieldSymbol ReferencedSymbol { get; }
        public IExpression? DefaultValue { get; }

        public FieldParameter(
            TextSpan span,
            FieldSymbol referencedSymbol,
            IExpression? defaultValue)
            : base(span, false)
        {
            ReferencedSymbol = referencedSymbol;
            DefaultValue = defaultValue;
        }
    }
}
