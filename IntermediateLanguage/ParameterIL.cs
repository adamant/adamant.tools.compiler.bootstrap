using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(SelfParameterIL),
        typeof(NamedParameterIL),
        typeof(FieldParameterIL))]
    public abstract class ParameterIL
    {
        public BindingSymbol? Symbol { get; }
        public bool IsMutableBinding { get; }
        public DataType DataType { get; internal set; }
        public FieldSymbol? InitializeField { get; }

        protected ParameterIL(
            BindingSymbol? symbol,
            bool isMutableBinding,
            DataType type,
            FieldSymbol? initializeField = null)
        {
            Symbol = symbol;
            IsMutableBinding = isMutableBinding;
            DataType = type;
            InitializeField = initializeField;
        }
    }
}
