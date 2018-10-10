using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public abstract class Declaration
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public FullyQualifiedName FullyQualifiedName { get; }

        protected Declaration(
            [NotNull] CodeFile file,
            [NotNull] FullyQualifiedName fullyQualifiedName)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(fullyQualifiedName), fullyQualifiedName);
            File = file;
            FullyQualifiedName = fullyQualifiedName;
        }
    }
}
