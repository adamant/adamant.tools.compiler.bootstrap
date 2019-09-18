using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        //typeof(TraitDeclarationSyntax),
        //typeof(StructDeclarationSyntax),
        //typeof(EnumClassDeclarationSyntax),
        //typeof(EnumStructDeclarationSyntax),
        typeof(ClassDeclarationSyntax))]
    public abstract class TypeDeclarationSyntax : MemberDeclarationSyntax
    {
        //public FixedList<GenericParameterSyntax> GenericParameters { get; }
        public FixedList<MemberDeclarationSyntax> Members { get; }
        //public Metatype Metatype => (Metatype)Type.Fulfilled();
        public DataType DeclaresType { get; }

        protected TypeDeclarationSyntax(
            CodeFile file,
            TextSpan nameSpan,
            Name fullName,
            //FixedList<GenericParameterSyntax> genericParameters,
            FixedList<MemberDeclarationSyntax> members)
            : base(file, fullName, nameSpan, new SymbolSet(members))
        {
            Members = members;
            //GenericParameters = genericParameters;
            foreach (var member in Members)
                member.DeclaringType = this;
        }
    }
}
