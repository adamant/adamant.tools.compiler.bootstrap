using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class Parameter
    {
        public bool MutableBinding { get; }
        [NotNull] public string Name { get; }
        [CanBeNull] public DataType Type { get; internal set; }

        public Parameter(bool mutableBinding, [NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            MutableBinding = mutableBinding;
            Name = name;
        }
    }
}
