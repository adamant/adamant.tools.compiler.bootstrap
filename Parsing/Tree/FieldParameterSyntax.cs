using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
    {
        private bool? isMutableBinding;
        public override bool IsMutableBinding => isMutableBinding ?? throw new Exception($"Tried to access {nameof(IsMutableBinding)} of field parameter before it had been assigned.");
        public new Name Name { get; }
        public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
        protected override IPromise<BindingSymbol> SymbolPromise => Symbol;
        public SimpleName FieldName { get; }

        public IExpressionSyntax? DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            MaybeQualifiedName fullName,
            Name name,
            IExpressionSyntax? defaultValue)
            : base(span, fullName, name)
        {
            FieldName = fullName.UnqualifiedName;
            Name = name;
            DefaultValue = defaultValue;
        }

        public void SetIsMutableBinding(bool value)
        {
            if (isMutableBinding != null) throw new InvalidOperationException("Can't set IsMutableBinding repeatedly");
            isMutableBinding = value;
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $".{Name}{defaultValue}";
        }
    }
}
