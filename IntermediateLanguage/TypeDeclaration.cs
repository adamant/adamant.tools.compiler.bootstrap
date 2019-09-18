using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class TypeDeclaration : Declaration
    {
        //public FixedList<GenericParameter> GenericParameters { get; }
        //public bool IsGeneric => GenericParameters != null;
        //public int? GenericArity => GenericParameters?.Count;
        public FixedList<Declaration> Members { get; }

        public TypeDeclaration(
            Name name,
            DataType type,
            //IEnumerable<GenericParameter> genericParameters,
            FixedList<Declaration> members)
            : base(false, name, type, new SymbolSet(members))
        {
            Members = members;
            //GenericParameters = genericParameters?.ToFixedList();
        }
    }
}
