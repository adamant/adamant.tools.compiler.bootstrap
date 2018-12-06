using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class GenericParameter
    {
        public Name Name { get; }
        public DataType Type { get; internal set; }

        public GenericParameter(
            Name name,
            DataType type)
        {
            Name = name;
            Type = type;
        }
    }
}
