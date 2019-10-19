using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing
{
    public abstract class BindingScope
    {
        public bool Lookup(SimpleName name, [NotNullWhen(true)] out VariableBinding? binding)
        {
            return LookupWithoutNumber(name.WithoutNumber(), out binding);
        }

        protected abstract bool LookupWithoutNumber(SimpleName name, [NotNullWhen(true)] out VariableBinding? binding);

        /// <summary>
        /// Indicates that some nested scope declared a variable binding.
        /// </summary>
        protected internal abstract void NestedBindingDeclared(VariableBinding binding);
    }
}
