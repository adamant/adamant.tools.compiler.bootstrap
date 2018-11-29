using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class IntegerNumericSymbol : ISymbol
    {
        [NotNull] private readonly SizedIntegerType numericType;
        [NotNull] private readonly PrimitiveMemberSymbol remainderMethod;
        DataType ISymbol.Type => numericType;

        public IntegerNumericSymbol([NotNull] SizedIntegerType numericType)
        {
            this.numericType = numericType;
            remainderMethod = new PrimitiveMemberSymbol("remainder", new FunctionType(new[] { numericType }, numericType));
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
