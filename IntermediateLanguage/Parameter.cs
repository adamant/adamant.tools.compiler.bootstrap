using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class Parameter
    {
        public bool MutableBinding { get; }
        [NotNull] public Name Name { get; }
        [NotNull] public DataType Type { get; internal set; }

        public Parameter(
            bool mutableBinding,
            [NotNull] Name name,
            [NotNull] DataType type)
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(type), type);
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
        }
    }
}
