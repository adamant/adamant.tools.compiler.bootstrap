using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public abstract class BlockTerminator
    {
        [NotNull]
        public abstract IEnumerable<int> OutBlocks();
    }
}
