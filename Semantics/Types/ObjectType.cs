using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    // Object types are the types created with class and struct declarations. The
    // primitive types are also object types. An object type may have generic parameters
    // that may be filled with generic arguments. An object type with generic parameters
    // but no generic arguments is *open* or *unbound*. One with generic arguments supplied
    // for all parameters is *closed* or *bound*. One with some but not all
    // arguments supplied is *partially bound*.
    public class ObjectType : DataType
    {
        [NotNull] public static readonly ObjectType Void = new ObjectType(new QualifiedName(new SimpleName("void", true)), false);
        [NotNull] public static readonly ObjectType Never = new ObjectType(new QualifiedName(new SimpleName("never", true)), false);
        [NotNull] public static readonly ObjectType Int = new ObjectType(new QualifiedName(new SimpleName("int", true)), false);
        [NotNull] public static readonly ObjectType UInt = new ObjectType(new QualifiedName(new SimpleName("uint", true)), false);
        [NotNull] public static readonly ObjectType Byte = new ObjectType(new QualifiedName(new SimpleName("byte", true)), false);
        [NotNull] public static readonly ObjectType Size = new ObjectType(new QualifiedName(new SimpleName("size", true)), false);
        [NotNull] public static readonly ObjectType Bool = new ObjectType(new QualifiedName(new SimpleName("bool", true)), false);
        [NotNull] public static readonly ObjectType String = new ObjectType(new QualifiedName(new SimpleName("string", true)), false);
        [NotNull] public static readonly ObjectType Type = new ObjectType(new QualifiedName(new SimpleName("type", true)), false);
        [NotNull] public static readonly ObjectType Metatype = new ObjectType(new QualifiedName(new SimpleName("metatype", true)), false);

        [NotNull] public QualifiedName Name { get; }
        public bool IsMutable { get; }

        public ObjectType([NotNull] QualifiedName name, bool isMutable)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            IsMutable = isMutable;
        }

        [NotNull]
        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name;

            return Name.ToString();
        }
    }
}
