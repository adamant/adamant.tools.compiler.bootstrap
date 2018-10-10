using Adamant.Tools.Compiler.Bootstrap.Old.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.Emit
{
    public abstract class Emitter
    {
        public abstract string Emit(Package package);
    }
}
