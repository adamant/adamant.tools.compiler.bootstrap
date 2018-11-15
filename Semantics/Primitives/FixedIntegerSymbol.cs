using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Primitives
{
    internal class FixedIntegerSymbol : ISymbol
    {
        [NotNull] private readonly PrimitiveFixedIntegerType type;
        [NotNull] private readonly PrimitiveMemberSymbol remainderMethod;

        public FixedIntegerSymbol([NotNull] PrimitiveFixedIntegerType type)
        {
            this.type = type;
            remainderMethod = new PrimitiveMemberSymbol("remainder", new FunctionType(new[] { type }, type));
        }

        public Name Name => type.Name;

        public IEnumerable<DataType> Types => type.Yield();

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
