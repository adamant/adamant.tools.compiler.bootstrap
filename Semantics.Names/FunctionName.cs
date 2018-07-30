using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class FunctionName : ScopeName
    {
        public ScopeName Scope { get; }
        public int Arity { get; }

        public FunctionName(ScopeName scope, string name, int arity)
            : base(name)
        {
            Scope = scope;
            Arity = arity;
        }

        public override void GetFullName(StringBuilder builder)
        {
            Scope.GetFullNameScope(builder);
            builder.Append(EntityName);
            builder.Append($"${Arity}()");
        }
    }
}
