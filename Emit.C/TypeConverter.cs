using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class TypeConverter : IConverter<DataType>
    {
        [NotNull] private readonly NameMangler nameMangler;

        public TypeConverter([NotNull] NameMangler nameMangler)
        {
            this.nameMangler = nameMangler;
        }

        public string Convert([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            switch (type)
            {
                // TODO perhaps the name mangler should be used on primitives
                case var t when t == ObjectType.Void:
                    return "void";
                case var t when t == ObjectType.Int:
                    return "ₐint";
                case ObjectType t:
                    return nameMangler.Mangle(t);
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
