using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Parameter
    {
        public bool IsMutableBinding { get; }
        public Name Name { get; }
        public DataType Type { get; internal set; }

        public Parameter(
            bool isMutableBinding,
            Name name,
            DataType type)
        {
            IsMutableBinding = isMutableBinding;
            Name = name;
            Type = type;
        }
    }
}
