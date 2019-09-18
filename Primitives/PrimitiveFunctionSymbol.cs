using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveFunctionSymbol : PrimitiveSymbol, IFunctionSymbol
    {
        public PrimitiveFunctionSymbol(
            Name fullName,
            FunctionType type,
            IEnumerable<ISymbol> childSymbols = null)
            : base(fullName, type, null, childSymbols)
        {
        }
    }
}
