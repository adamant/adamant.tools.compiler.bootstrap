using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class PackageName : Name
    {
        public PackageName([NotNull] string name)
            : base(name)
        {
            // TODO validate that the name is a valid package name
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            builder.Append(EntityName);
            builder.Append(":"); // One colon is a bare package name, the second colon indicates the global namespace
        }

        public override void GetFullNameScope([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            GetFullName(builder);
        }
    }
}
