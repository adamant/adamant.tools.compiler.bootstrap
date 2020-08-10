using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamedParameterSyntax : ParameterSyntax, INamedParameterSyntax
    {
        public override bool IsMutableBinding { get; }
        public new Name Name { get; }
        public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
        protected override IPromise<BindingSymbol> SymbolPromise => Symbol;
        public ITypeSyntax Type { get; }
        public IExpressionSyntax? DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isMutableBinding,
            MaybeQualifiedName fullName,
            ITypeSyntax typeSyntax,
            IExpressionSyntax? defaultValue)
            : base(span, fullName, fullName.UnqualifiedName.Text)
        {
            IsMutableBinding = isMutableBinding;
            Name = fullName.UnqualifiedName.Text;
            Type = typeSyntax;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $"{Name}: {Type}{defaultValue}";
        }
    }
}
