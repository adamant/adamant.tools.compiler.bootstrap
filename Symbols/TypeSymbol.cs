using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(PrimitiveTypeSymbol),
        typeof(ObjectTypeSymbol))]
    public abstract class TypeSymbol : Symbol
    {
        public new TypeName Name { get; }
        public DataType DeclaresDataType { get; }

        protected TypeSymbol(NamespaceOrPackageSymbol? containingSymbol, TypeName name, DataType declaresDataType)
            : base(containingSymbol, name)
        {
            Name = name;
            DeclaresDataType = declaresDataType;
        }
    }
}
