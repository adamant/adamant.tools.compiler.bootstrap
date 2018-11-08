using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public interface ILocalVariableScopeAnalysis
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<IDeclarationAnalysis> LocalVariableDeclarations();
    }
}
