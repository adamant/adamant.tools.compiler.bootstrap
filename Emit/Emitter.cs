using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.Emit
{
    public abstract class Emitter
    {
        public abstract void Emit(Package package);
        public abstract string GetEmittedCode();
    }
}
