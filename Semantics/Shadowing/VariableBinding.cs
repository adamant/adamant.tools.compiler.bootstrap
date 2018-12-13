using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing
{
    public class VariableBinding
    {
        public bool MutableBinding { get; }
        public SimpleName BindingName { get; }
        public SimpleName Name { get; }
        public TextSpan NameSpan { get; }
        public IReadOnlyList<VariableBinding> WasShadowedBy => wasShadowedBy;
        private readonly List<VariableBinding> wasShadowedBy = new List<VariableBinding>();

        public VariableBinding(ParameterSyntax parameter)
        {
            MutableBinding = parameter.MutableBinding;
            BindingName = parameter.Name;
            Name = BindingName.WithoutNumber();
            NameSpan = parameter.Span;
        }

        public VariableBinding(VariableDeclarationStatementSyntax variableDeclaration)
        {
            MutableBinding = variableDeclaration.MutableBinding;
            BindingName = variableDeclaration.Name;
            Name = BindingName.WithoutNumber();
            NameSpan = variableDeclaration.NameSpan;
        }

        public void AddShadowingBinding(VariableBinding binding)
        {
            if (Name == binding.Name)
                wasShadowedBy.Add(binding);
        }
    }
}
