using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ClassDeclaration : Declaration, ITypeMetadata
    {
        public FixedList<Declaration> Members { get; }
        public DataType DeclaresDataType { get; }

        public ClassDeclaration(
            MaybeQualifiedName name,
            DataType declaresType,
            FixedList<Declaration> members)
            : base(false, name, new MetadataSet(members))
        {
            DeclaresDataType = declaresType;
            Members = members;
        }
    }
}
