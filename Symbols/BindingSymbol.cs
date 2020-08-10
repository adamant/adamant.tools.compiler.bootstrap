using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(VariableSymbol),
        typeof(SelfParameterSymbol),
        typeof(FieldSymbol))]
    public abstract class BindingSymbol : Symbol
    {
        public override PackageSymbol? Package { get; }
        public new Name? Name { get; }
        public bool IsMutableBinding { get; }
        public DataType DataType { get; }

        protected BindingSymbol(Symbol containingSymbol, Name? name, bool isMutableBinding, DataType dataType)
            : base(containingSymbol, name)
        {
            Package = containingSymbol.Package;
            Name = name;
            IsMutableBinding = isMutableBinding;
            DataType = dataType;
        }
    }
}
