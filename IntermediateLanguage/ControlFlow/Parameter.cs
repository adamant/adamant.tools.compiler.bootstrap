using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Parameter
    {
        public bool MutableBinding { get; }
        public Name Name { get; }
        public DataType Type { get; internal set; }

        public Parameter(
            bool mutableBinding,
            Name name,
            DataType type)
        {
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
        }
    }
}
