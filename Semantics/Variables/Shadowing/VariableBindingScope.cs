using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing
{
    public class VariableBindingScope : BindingScope
    {
        public BindingScope ContainingScope { get; }
        public VariableBinding VariableBinding { get; }

        public VariableBindingScope(
            BindingScope containingScope,
            INamedParameter parameter)
        {
            ContainingScope = containingScope;
            VariableBinding = new VariableBinding(parameter);
            ContainingScope.NestedBindingDeclared(VariableBinding);
        }

        public VariableBindingScope(BindingScope containingScope,
            IVariableDeclarationStatement variableDeclaration)
        {
            ContainingScope = containingScope;
            VariableBinding = new VariableBinding(variableDeclaration);
            ContainingScope.NestedBindingDeclared(VariableBinding);
        }

        protected override bool LookupWithoutNumber(Name name, [NotNullWhen(true)] out VariableBinding? binding)
        {
            if (VariableBinding.Name != name)
                return ContainingScope.Lookup(name, out binding);

            binding = VariableBinding;
            return true;
        }

        protected internal override void NestedBindingDeclared(VariableBinding binding)
        {
            VariableBinding.NestedBindingDeclared(binding);
            ContainingScope.NestedBindingDeclared(binding);
        }
    }
}
