using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class MemberDeclarationSyntax : DeclarationSyntax, IMemberDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public IAccessModifierToken? AccessModifier { get; }
        public MaybeQualifiedName FullName { get; }
        public MetadataSet ChildMetadata { get; protected set; }

        protected MemberDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name? name,
            MetadataSet? childSymbols = null)
            : base(span, file, name, nameSpan)
        {
            DeclaringClass = declaringClass;
            AccessModifier = accessModifier;
            FullName = fullName;
            ChildMetadata = childSymbols ?? MetadataSet.Empty;
        }
    }
}
