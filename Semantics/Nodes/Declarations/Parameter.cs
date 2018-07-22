using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class Parameter : Declaration
    {
        public new ParameterSyntax Syntax { get; }
        public bool MutableBinding { get; }
        public string Name { get; }
        public TypeName Type { get; }

        public Parameter(ParameterSyntax syntax, bool mutableBinding, string name, TypeName type)
        {
            Syntax = syntax;
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
