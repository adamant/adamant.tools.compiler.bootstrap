using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    [Closed(
        typeof(SelfParameter),
        typeof(NamedParameter),
        typeof(FieldParameter))]
    public abstract class Parameter
    {
        public BindingSymbol? Symbol { get; }
        public bool IsMutableBinding { get; }
        public DataType DataType { get; internal set; }
        public FieldSymbol? InitializeField { get; }

        protected Parameter(
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
