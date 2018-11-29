using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class IntegerNumericSymbol : ISymbol
    {
        public Name FullName => NumericType.Name;
        [NotNull] public DataType Type { get; }
        [NotNull] public readonly IntegerType NumericType;
        [NotNull] public FixedDictionary<SimpleName, ISymbol> ChildSymbols { get; }

        public IntegerNumericSymbol([NotNull] IntegerType numericType)
        {
            NumericType = numericType;
            var children = ConstructChildSymbols(numericType);
            ChildSymbols = children.ToFixedDictionary(s => s.FullName.UnqualifiedName);
            Type = new Metatype(this, numericType);
        }

        [NotNull]
        private static List<ISymbol> ConstructChildSymbols([NotNull] IntegerType numericType)
        {
            return new List<ISymbol>
            {
                new PrimitiveMemberSymbol("remainder", new FunctionType(new[] {numericType}, numericType))
            };
        }
    }
}
