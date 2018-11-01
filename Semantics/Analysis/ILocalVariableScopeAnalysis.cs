using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public interface ILocalVariableScopeAnalysis
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<IDeclarationAnalysis> LocalVariableDeclarations();
    }
}
