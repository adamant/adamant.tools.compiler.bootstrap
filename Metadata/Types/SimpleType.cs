using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class SimpleType : DataType
    {
        [NotNull] public Name Name { get; }

        protected SimpleType([NotNull] string name)
        {
            Name = SimpleName.Special(name);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
