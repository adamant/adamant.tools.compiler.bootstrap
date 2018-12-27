using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// A meta-function is a function that operates at the meta level. That is,
    /// it is "evaluated" at compile time. A type constructor has a meta-function
    /// type. For example, a class `Foo[T]` has the type `[type] -> type`.
    /// However, generic functions also have meta-function types. For example,
    /// the `size_of` function which has the type `[type] -> size`.
    /// </summary>
    public class MetaFunctionType : ReferenceType
    {
        public FixedList<DataType> ParameterTypes { get; }
        public int Arity => ParameterTypes.Count;
        public DataType ResultType { get; }
        public override bool IsResolved { get; }

        public MetaFunctionType(
            IEnumerable<DataType> parameterTypes,
            DataType resultType)
            : base(Lifetime.Forever)
        {
            ParameterTypes = parameterTypes.ToFixedList();
            ResultType = resultType;
            IsResolved = ParameterTypes.All(pt => pt.IsResolved) && ResultType.IsResolved;
        }

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", ParameterTypes)}] -> {ResultType}";
        }
    }
}
