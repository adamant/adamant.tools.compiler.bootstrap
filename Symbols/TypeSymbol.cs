using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
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
