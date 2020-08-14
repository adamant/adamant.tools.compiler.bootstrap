using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
    {
        public new Name Name { get; }
        public Promise<FieldSymbol?> ReferencedSymbol { get; } = new Promise<FieldSymbol?>();
        public override IPromise<DataType> DataType { get; }
        public IExpressionSyntax? DefaultValue { get; }

        public FieldParameterSyntax(TextSpan span, Name name, IExpressionSyntax? defaultValue)
            : base(span, name)
        {
            Name = name;
            DefaultValue = defaultValue;
            DataType = ReferencedSymbol.Select(s => s?.DataType ?? Types.DataType.Unknown);
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $".{Name}{defaultValue}";
        }
    }
}
