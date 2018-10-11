using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class TypeConverter : IConverter<DataType>
    {
        public string Convert([NotNull] DataType type)
        {
            switch (type)
            {
                // TODO perhaps the name mangler should be used on primitives
                case var t when t == ObjectType.Void:
                    return "void";
                case var t when t == ObjectType.Int:
                    return "ₐint";
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
