using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
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
                case var t when t == DataType.Void:
                    return "void";
                case SizedIntegerType integerType:
                    return nameMangler.Mangle(integerType.Name);
                case ObjectType t:
                    return nameMangler.Mangle(t);
                case LifetimeType lifetimeType:
                    return Convert(lifetimeType.Referent);
                case PointerType ptr:
                    var referenced = ptr.Referent.AssertResolved();
                    if (referenced == DataType.Any)
                        return "void*";

                    return Convert(referenced) + "*";
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
