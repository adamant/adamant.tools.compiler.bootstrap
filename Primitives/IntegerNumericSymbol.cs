using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class IntegerNumericSymbol : ISymbol
    {
        [NotNull] private readonly IntegerType numericType;
        [NotNull] private readonly PrimitiveMemberSymbol remainderMethod;
        public DataType Type { get; }

        public IntegerNumericSymbol([NotNull] IntegerType numericType)
        {
            this.numericType = numericType;
            remainderMethod = new PrimitiveMemberSymbol("remainder", new FunctionType(new[] { numericType }, numericType));
            Type = new Metatype(this, numericType);
        }

        public Name FullName => numericType.Name;

        public ISymbol ComposeWith(ISymbol symbol)
        {
            throw new NotSupportedException();
        }

        public ISymbol Lookup(SimpleName name)
        {
            if (name.IsSpecial) return null;

            switch (name.Text)
            {
                case "remainder":
                    return remainderMethod;
                default:
                    return null;
            }
        }
    }
}
