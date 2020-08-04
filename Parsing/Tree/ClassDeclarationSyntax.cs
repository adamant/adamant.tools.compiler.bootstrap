using System;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ClassDeclarationSyntax : DeclarationSyntax, IClassDeclarationSyntax
    {
        public IAccessModifierToken? AccessModifier { get; }
        public IMutableKeywordToken? MutableModifier { get; }
        public MaybeQualifiedName FullName { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public FixedList<IMemberDeclarationSyntax> Members { get; }
        public DataTypePromise DeclaresDataType { get; } = new DataTypePromise();
        public MetadataSet ChildMetadata { get; protected set; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ITypeMetadata.DeclaresDataType => DeclaresDataType.Fulfilled();

        public ClassDeclarationSyntax(
            TextSpan headerSpan,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            IMutableKeywordToken? mutableModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Func<IClassDeclarationSyntax, (FixedList<IMemberDeclarationSyntax>, TextSpan)> parseMembers)
            : base(headerSpan, file, nameSpan)
        {
            AccessModifier = accessModifier;
            MutableModifier = mutableModifier;
            FullName = fullName;
            var (members, bodySpan) = parseMembers(this);
            Members = members;
            ChildMetadata = new MetadataSet(Members);
            Span = TextSpan.Covering(headerSpan, bodySpan);
        }

        public void CreateDefaultConstructor()
        {
            if (Members.Any(m => m is IConstructorDeclarationSyntax))
                return;

            var constructor = new DefaultConstructor((ObjectType)DeclaresDataType.Fulfilled());
            ChildMetadata = new MetadataSet(ChildMetadata.Append<IMetadata>(constructor));
        }

        public override string ToString()
        {
            return $"class {FullName} {{ … }}";
        }
    }
}
