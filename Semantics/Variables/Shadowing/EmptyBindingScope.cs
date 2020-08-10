using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing
{
    public class EmptyBindingScope : BindingScope
    {
        #region Singleton
        public static readonly BindingScope Instance = new EmptyBindingScope();

        private EmptyBindingScope() { }
        #endregion

        protected override bool LookupWithoutNumber(SimpleName name, [NotNullWhen(true)] out VariableBinding? binding)
        {
            binding = null;
            return false;
        }

        protected internal override void NestedBindingDeclared(VariableBinding binding)
        {
            // Empty scope has no bindings, so nested bindings don't matter
        }
    }
}
