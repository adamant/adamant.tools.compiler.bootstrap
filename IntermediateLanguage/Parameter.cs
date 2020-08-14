using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class Parameter
    {
        public bool IsMutableBinding { get; }
        public SimpleName Name { get; }
        public DataType DataType { get; internal set; }

        public Parameter(
            bool isMutableBinding,
            SimpleName name,
            DataType type)
        {
            IsMutableBinding = isMutableBinding;
            Name = name;
            DataType = type;
        }
    }
}
