using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class MemberDeclarationSyntax : DeclarationSyntax, IMemberDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public IAccessModifierToken? AccessModifier { get; }
        public MaybeQualifiedName FullName { get; }

        protected MemberDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name? name,
            IPromise<Symbol> symbol)
            : base(span, file, name, nameSpan, symbol)
        {
            DeclaringClass = declaringClass;
            AccessModifier = accessModifier;
            FullName = fullName;
        }
    }
}
