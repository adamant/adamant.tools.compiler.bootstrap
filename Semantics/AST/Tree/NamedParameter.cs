using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NamedParameter : Parameter, INamedParameter
    {
        public VariableSymbol Symbol { get; }
        BindingSymbol IBinding.Symbol => Symbol;
        NamedBindingSymbol ILocalBinding.Symbol => Symbol;
        public IExpression? DefaultValue { get; }

        public NamedParameter(
            TextSpan span,
            VariableSymbol symbol,
            bool unused,
            IExpression? defaultValue)
            : base(span, unused)
        {
            Symbol = symbol;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            var mutable = Symbol.IsMutableBinding ? "mut " : "";
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            var declarationNumber = Symbol.DeclarationNumber is null ? "" : "#" + Symbol.DeclarationNumber;
            return $"{mutable}{Symbol.Name}{declarationNumber}: {Symbol.DataType}{defaultValue}";
        }
    }
}
