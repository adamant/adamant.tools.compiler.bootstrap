using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public sealed class FieldParameter : Parameter
    {
        public new FieldSymbol InitializeField { get; }

        public FieldParameter(FieldSymbol initializeField)
            : base(null, false, initializeField.DataType, initializeField)
        {
            InitializeField = initializeField;
        }
    }
}
