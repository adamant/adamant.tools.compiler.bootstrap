using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    public class PrimitiveMemberSymbol : ISymbol
    {
        public Name FullName { get; }
        [NotNull] private readonly FunctionType type;
        DataType ISymbol.Type => type;

        public PrimitiveMemberSymbol([NotNull] string name, [NotNull] FunctionType type)
        {
            FullName = new SimpleName(name);
            this.type = type;
        }

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
