using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.Emit
{
    public abstract class Emitter
    {
        public abstract void Emit(PackageIL package);
        public abstract string GetEmittedCode();
    }
}
