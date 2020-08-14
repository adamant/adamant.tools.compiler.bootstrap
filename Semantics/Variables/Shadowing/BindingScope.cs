using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing
{
    public abstract class BindingScope
    {
        public bool Lookup(Name name, [NotNullWhen(true)] out VariableBinding? binding)
        {
            return LookupWithoutNumber(name, out binding);
        }

        protected abstract bool LookupWithoutNumber(Name name, [NotNullWhen(true)] out VariableBinding? binding);

        /// <summary>
        /// Indicates that some nested scope declared a variable binding.
        /// </summary>
        protected internal abstract void NestedBindingDeclared(VariableBinding binding);
    }
}
