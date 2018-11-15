using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Primitives
{
    public class PrimitiveMemberSymbol : ISymbol
    {
        public Name Name { get; }
        private readonly FunctionType type;

        public PrimitiveMemberSymbol([NotNull] string name, [NotNull] FunctionType type)
        {
            Name = new SimpleName(name);
            this.type = type;
        }

        public IEnumerable<DataType> Types => type.Yield();

        public ISymbol ComposeWith(ISymbol symbol)
        {
            throw new NotSupportedException();
        }

        public ISymbol Lookup(SimpleName name)
        {
            return null;
        }
    }
}
