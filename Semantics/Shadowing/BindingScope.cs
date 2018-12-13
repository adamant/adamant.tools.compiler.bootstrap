using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing
{
    public abstract class BindingScope
    {
        public bool Lookup(SimpleName name, out VariableBinding binding)
        {
            return LookupWithoutNumber(name.WithoutNumber(), out binding);
        }

        protected abstract bool LookupWithoutNumber(SimpleName name, out VariableBinding binding);

        protected internal abstract void AddShadowingBinding(VariableBinding binding);
    }
}
