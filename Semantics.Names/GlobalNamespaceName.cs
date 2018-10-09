using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class GlobalNamespaceName : NamespaceName
    {
        [NotNull] public PackageName Package { get; }
        public override bool IsGlobalNamespace => true;

        public GlobalNamespaceName([NotNull] PackageName package)
        : base("")
        {
            Requires.NotNull(nameof(package), package);
            Package = package;
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            Package.GetFullNameScope(builder);
            builder.Append(":"); // The first colon comes from the package name
        }

        public override void GetFullNameScope([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            GetFullName(builder);
        }
    }
}
