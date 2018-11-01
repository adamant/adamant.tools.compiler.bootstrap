using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Inheritance;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types
{
    public class StructDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public StructKeywordToken StructKeyword { get; }
        [NotNull] public IIdentifierOrPrimitiveToken Name { get; }
        [CanBeNull] public GenericParametersSyntax GenericParameters { get; }
        [CanBeNull] public BaseTypesSyntax BaseTypes { get; }
        [NotNull] public SyntaxList<GenericConstraintSyntax> GenericConstraints { get; }
        [NotNull] public IOpenBraceToken OpenBrace { get; }
        [NotNull] public SyntaxList<MemberDeclarationSyntax> Members { get; }
        [NotNull] public ICloseBraceToken CloseBrace { get; }

        public StructDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] StructKeywordToken structKeyword,
            [NotNull] IIdentifierOrPrimitiveToken name,
            [CanBeNull] GenericParametersSyntax genericParameters,
            [CanBeNull] BaseTypesSyntax baseTypes,
            [NotNull] SyntaxList<GenericConstraintSyntax> genericConstraints,
            [NotNull] IOpenBraceToken openBrace,
            [NotNull] SyntaxList<MemberDeclarationSyntax> members,
            [NotNull] ICloseBraceToken closeBrace)
            : base(TextSpan.Covering(structKeyword.Span, name.Span))
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(structKeyword), structKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(members), members);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Modifiers = modifiers;
            StructKeyword = structKeyword;
            Name = name;
            GenericParameters = genericParameters;
            BaseTypes = baseTypes;
            GenericConstraints = genericConstraints;
            OpenBrace = openBrace;
            Members = members;
            CloseBrace = closeBrace;
        }
    }
}
