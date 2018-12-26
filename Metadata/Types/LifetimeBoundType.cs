using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class LifetimeBoundType : DataType
    {
        public ObjectType Referent { get; }
        public LifetimeRelationOperator Operator { get; }
        public Lifetime Lifetime { get; }
        public override bool IsResolved { get; }
        public bool IsOwned => Lifetime.IsOwned;

        public LifetimeBoundType(ObjectType referent, LifetimeRelationOperator @operator, Lifetime lifetime)
        {
            Referent = referent;
            Operator = @operator;
            Lifetime = lifetime;
            IsResolved = Referent.IsResolved;
        }

        public override string ToString()
        {
            return $"{Referent}${Lifetime}";
        }
    }
}
