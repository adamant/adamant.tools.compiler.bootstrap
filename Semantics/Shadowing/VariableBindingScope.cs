using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing
{
    public class VariableBindingScope : BindingScope
    {
        public BindingScope ContainingScope { get; }
        public VariableBinding VariableBinding { get; }

        public VariableBindingScope(BindingScope containingScope, IParameterSyntax parameter)
        {
            ContainingScope = containingScope;
            VariableBinding = new VariableBinding(parameter);
            ContainingScope.AddShadowingBinding(VariableBinding);
        }

        public VariableBindingScope(BindingScope containingScope, VariableDeclarationStatementSyntax variableDeclaration)
        {
            ContainingScope = containingScope;
            VariableBinding = new VariableBinding(variableDeclaration);
            ContainingScope.AddShadowingBinding(VariableBinding);
        }

        protected override bool LookupWithoutNumber(SimpleName name, out VariableBinding binding)
        {
            if (VariableBinding.Name == name)
            {
                binding = VariableBinding;
                return true;
            }

            return ContainingScope.Lookup(name, out binding);
        }

        protected internal override void AddShadowingBinding(VariableBinding binding)
        {
            VariableBinding.AddShadowingBinding(binding);
            ContainingScope.AddShadowingBinding(binding);
        }
    }
}
