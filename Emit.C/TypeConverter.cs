using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class TypeConverter : IConverter<KnownType>
    {
        [NotNull] private readonly NameMangler nameMangler;

        public TypeConverter([NotNull] NameMangler nameMangler)
        {
            this.nameMangler = nameMangler;
        }

        public string Convert([NotNull] KnownType type)
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
                case LifetimeType lifetimeType:
                    return Convert(lifetimeType.Type);
                case PointerType ptr:
                    var referenced = ptr.ReferencedType.AssertKnown();
                    if (referenced == ObjectType.Any)
                        return "void*";

                    return Convert(referenced) + "*";
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }
    }
}
