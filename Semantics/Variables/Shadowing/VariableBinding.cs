using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing
{
    public class VariableBinding
    {
        public bool MutableBinding { get; }
        public Name Name { get; }
        public TextSpan NameSpan { get; }
        public IReadOnlyList<VariableBinding> WasShadowedBy => wasShadowedBy;
        private readonly List<VariableBinding> wasShadowedBy = new List<VariableBinding>();

        public VariableBinding(INamedParameter parameter)
        {
            MutableBinding = parameter.Symbol.IsMutableBinding;
            Name = parameter.Symbol.Name;
            NameSpan = parameter.Span;
        }

        public VariableBinding(IVariableDeclarationStatement variableDeclaration)
        {
            MutableBinding = variableDeclaration.Symbol.IsMutableBinding;
            Name = variableDeclaration.Symbol.Name;
            NameSpan = variableDeclaration.NameSpan;
        }

        public void NestedBindingDeclared(VariableBinding binding)
        {
            if (Name == binding.Name)
                wasShadowedBy.Add(binding);
        }
    }
}
