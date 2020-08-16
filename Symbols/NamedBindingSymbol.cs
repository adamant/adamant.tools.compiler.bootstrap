using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(VariableSymbol),
        typeof(FieldSymbol))]
    public abstract class NamedBindingSymbol : BindingSymbol
    {
        public new Name Name { get; }

        protected NamedBindingSymbol(
            Symbol containingSymbol,
            Name name,
            bool isMutableBinding,
            DataType dataType)
            : base(containingSymbol, name, isMutableBinding, dataType)
        {
            Name = name;
        }
    }
}
