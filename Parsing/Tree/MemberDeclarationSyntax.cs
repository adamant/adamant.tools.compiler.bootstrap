using System.Diagnostics;
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

        public Name FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public MetadataSet ChildMetadata { get; protected set; }

        protected MemberDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            Name fullName,
            TextSpan nameSpan,
            MetadataSet? childSymbols = null)
            : base(span, file, nameSpan)
        {
            DeclaringClass = declaringClass;
            AccessModifier = accessModifier;
            FullName = fullName;
            ChildMetadata = childSymbols ?? MetadataSet.Empty;
        }
    }
}
