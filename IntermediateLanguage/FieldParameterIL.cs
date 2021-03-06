using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public sealed class FieldParameterIL : ParameterIL
    {
        public new FieldSymbol InitializeField { get; }

        public FieldParameterIL(FieldSymbol initializeField)
            : base(null, false, initializeField.DataType, initializeField)
        {
            InitializeField = initializeField;
        }
    }
}
