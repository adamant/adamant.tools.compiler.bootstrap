using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names
{
    public class VariableName : Name
    {
        [NotNull] public FunctionName Function { get; }
        // TODO add declaration number

        public VariableName([NotNull] FunctionName function, [NotNull] string name)
            : base(name)
        {
            Requires.NotNull(nameof(function), function);
            Function = function;
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            Function.GetFullNameScope(builder);
            builder.Append(EntityName);
        }
    }
}
