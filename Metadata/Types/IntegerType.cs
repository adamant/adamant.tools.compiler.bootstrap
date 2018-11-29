using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public abstract class IntegerType : NumericType
    {
        protected IntegerType([NotNull] string name)
            : base(name)
        {
        }
    }
}
