using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class FunctionName : ScopeName
    {
        [NotNull] public ScopeName Scope { get; }
        public int Arity { get; }

        public FunctionName([NotNull] ScopeName scope, [NotNull] string name, int arity)
            : base(name)
        {
            Requires.NotNull(nameof(scope), scope);
            Scope = scope;
            Arity = arity;
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            Scope.GetFullNameScope(builder);
            builder.Append(EntityName);
            builder.Append($"${Arity}()");
        }
    }
}
