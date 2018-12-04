using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class AccessorSymbol : Symbol, IAccessorSymbol
    {
        public Name PropertyName { get; }

        public AccessorSymbol(
            Name fullName,
            Name propertyName,
            DataType type,
            IEnumerable<ISymbol> childSymbols = null)
            : base(fullName, type, childSymbols)
        {
            PropertyName = propertyName;
        }
    }
}
