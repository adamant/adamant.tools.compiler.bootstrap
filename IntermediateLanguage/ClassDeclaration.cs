using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ClassDeclaration : Declaration, ITypeMetadata
    {
        public FixedList<Declaration> Members { get; }
        public DataType DeclaresType { get; }

        public ClassDeclaration(
            Name name,
            DataType declaresType,
            FixedList<Declaration> members)
            : base(false, name, new MetadataSet(members))
        {
            DeclaresType = declaresType;
            Members = members;
        }
    }
}
