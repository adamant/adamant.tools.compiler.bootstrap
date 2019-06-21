using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ClassDeclarationSyntax : TypeDeclarationSyntax
    {
        public FixedList<AttributeSyntax> Attributes { get; }
        public FixedList<IModiferToken> Modifiers { get; }
        public ExpressionSyntax BaseClass { get; }
        public FixedList<ExpressionSyntax> BaseTypes { get; }
        public FixedList<GenericConstraintSyntax> GenericConstraints { get; }
        public FixedList<ExpressionSyntax> Invariants { get; }

        public ClassDeclarationSyntax(
            CodeFile file,
            FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<GenericParameterSyntax> genericParameters,
            ExpressionSyntax baseClass,
            FixedList<ExpressionSyntax> baseTypes,
            FixedList<GenericConstraintSyntax> genericConstraints,
            FixedList<ExpressionSyntax> invariants,
            FixedList<MemberDeclarationSyntax> members)
            : base(file, nameSpan, fullName, genericParameters, members)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            BaseClass = baseClass;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            Invariants = invariants;
        }

        public void CreateDefaultConstructor()
        {
            if (Members.Any(m => m is ConstructorDeclarationSyntax)) return;
            var metatype = (Metatype)Type.Known();
            var constructor = new DefaultConstructor((UserObjectType)metatype.Instance);
            ChildSymbols = new SymbolSet(ChildSymbols.Values.SelectMany(s => s).Append(constructor));
        }

        public override string ToString()
        {
            var generics = GenericParameters != null
                ? $"[{string.Join(", ", GenericParameters)}]"
                : "";
            return $"class {FullName}{generics} {{ … }}";
        }
    }
}
