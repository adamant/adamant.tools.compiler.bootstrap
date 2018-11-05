using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public abstract class Declaration : ISymbol
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public QualifiedName Name { get; }
        [NotNull] public DataType Type { get; } // TODO that should be a known type

        protected Declaration(
            [NotNull] CodeFile file,
            [NotNull] QualifiedName name,
            [NotNull] KnownType type)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(type), type);
            File = file;
            Name = name;
            Type = type;
        }
    }
}
