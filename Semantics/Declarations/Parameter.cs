using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class Parameter
    {
        public bool MutableBinding { get; }
        [NotNull] public Name Name { get; }
        [NotNull] public KnownType Type { get; internal set; }

        public Parameter(
            bool mutableBinding,
            [NotNull] Name name,
            [NotNull] KnownType type)
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(type), type);
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
        }
    }
}
