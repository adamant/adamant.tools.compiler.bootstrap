using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
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
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
        }
    }
}
