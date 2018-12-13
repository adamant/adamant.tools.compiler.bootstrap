using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing
{
    public class EmptyBindingScope : BindingScope
    {
        #region Singleton
        public static readonly BindingScope Instance = new EmptyBindingScope();

        private EmptyBindingScope() { }
        #endregion

        protected override bool LookupWithoutNumber(SimpleName name, out VariableBinding binding)
        {
            binding = null;
            return false;
        }

        protected internal override void AddShadowingBinding(VariableBinding binding)
        {
        }
    }
}
