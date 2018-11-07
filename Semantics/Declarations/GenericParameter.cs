using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class GenericParameter
    {
        [NotNull] public Name Name { get; }
        [NotNull] public KnownType Type { get; internal set; }

        public GenericParameter(
            [NotNull] Name name,
            [NotNull] KnownType type)
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(type), type);
            Name = name;
            Type = type;
        }
    }
}
