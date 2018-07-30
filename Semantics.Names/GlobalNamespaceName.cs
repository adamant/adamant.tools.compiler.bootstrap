using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class GlobalNamespaceName : NamespaceName
    {
        public PackageName Package { get; }
        public override bool IsGlobalNamespace => true;

        public GlobalNamespaceName(PackageName package)
        : base("")
        {
            Package = package;
        }

        public override void GetFullName(StringBuilder builder)
        {
            Package.GetFullNameScope(builder);
            builder.Append(":"); // The first colon comes from the package name
        }

        public override void GetFullNameScope(StringBuilder builder)
        {
            GetFullName(builder);
        }
    }
}
