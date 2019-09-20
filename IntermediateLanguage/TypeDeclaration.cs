using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class TypeDeclaration : Declaration, ITypeSymbol
    {
        public FixedList<Declaration> Members { get; }
        public DataType DeclaresType { get; }

        public TypeDeclaration(
            Name name,
            DataType declaresType,
            FixedList<Declaration> members)
            : base(false, name, new SymbolSet(members))
        {
            DeclaresType = declaresType;
            Members = members;
        }
    }
}
