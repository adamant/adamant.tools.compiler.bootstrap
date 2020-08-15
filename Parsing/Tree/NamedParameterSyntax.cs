using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamedParameterSyntax : ParameterSyntax, INamedParameterSyntax
    {
        public bool IsMutableBinding { get; }
        public new Name Name { get; }
        public Promise<int?> DeclarationNumber { get; } = new Promise<int?>();
        public Promise<VariableSymbol> Symbol { get; } = new Promise<VariableSymbol>();
        IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
        public ITypeSyntax Type { get; }
        public override IPromise<DataType> DataType { get; }
        public IExpressionSyntax? DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name name,
            ITypeSyntax typeSyntax,
            IExpressionSyntax? defaultValue)
            : base(span, name)
        {
            IsMutableBinding = isMutableBinding;
            Name = name;
            Type = typeSyntax;
            DefaultValue = defaultValue;
            DataType = Symbol.Select(s => s.DataType);
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $"{Name}: {Type}{defaultValue}";
        }
    }
}
