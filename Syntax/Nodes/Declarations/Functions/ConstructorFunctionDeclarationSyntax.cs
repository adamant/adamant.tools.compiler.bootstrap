using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Clauses;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions
{
    public class ConstructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public NewKeywordToken NewKeyword { get; }
        [CanBeNull] public override IIdentifierToken Name { get; }

        public ConstructorFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] NewKeywordToken newKeyword,
            [CanBeNull] IdentifierToken name,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenToken closeParen,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] BlockExpressionSyntax body)
            : base(modifiers, openParen, parameterList, closeParen, effects, body)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(newKeyword), newKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(body), body);
            NewKeyword = newKeyword;
            Name = name;
        }
    }
}
