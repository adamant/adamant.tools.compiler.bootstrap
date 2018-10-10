using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.Emit
{
    public abstract class Emitter
    {
        public abstract string Emit(Package package);
    }
}
