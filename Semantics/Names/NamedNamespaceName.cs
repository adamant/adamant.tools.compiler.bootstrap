using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class NamedNamespaceName : NamespaceName
    {
        public NamespaceName ContainingNamespace { get; }
        public override bool IsGlobalNamespace => false;

        public NamedNamespaceName(NamespaceName containingNamespace, string name)
            : base(name)
        {
            // TODO validate that it is a legal namespace name
            ContainingNamespace = containingNamespace;
        }

        public override void GetFullName(StringBuilder builder)
        {
            ContainingNamespace.GetFullNameScope(builder);
            builder.Append(EntityName);
        }
    }
}
