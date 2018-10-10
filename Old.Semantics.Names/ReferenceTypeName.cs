using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names
{
    public class ReferenceTypeName : ObjectTypeName
    {
        [NotNull] public ScopeName Scope { get; }

        public ReferenceTypeName([NotNull] ScopeName scope, [NotNull] string name)
            : base(name)
        {
            Requires.NotNull(nameof(scope), scope);
            Scope = scope;
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            Scope.GetFullNameScope(builder);
            builder.Append(EntityName);
        }
    }
}
