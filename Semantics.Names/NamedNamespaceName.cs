using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class NamedNamespaceName : NamespaceName
    {
        [NotNull] public NamespaceName ContainingNamespace { get; }
        public override bool IsGlobalNamespace => false;

        public NamedNamespaceName([NotNull] NamespaceName containingNamespace, [NotNull] string name)
            : base(name)
        {
            Requires.NotNull(nameof(containingNamespace), containingNamespace);
            // TODO validate that it is a legal namespace name
            ContainingNamespace = containingNamespace;
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            ContainingNamespace.GetFullNameScope(builder);
            builder.Append(EntityName);
        }
    }
}
