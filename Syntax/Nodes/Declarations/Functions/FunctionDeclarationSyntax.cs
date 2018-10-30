using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Clauses;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions
{
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ParameterSyntax> ParameterList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ParameterSyntax> Parameters => ParameterList.Nodes();
        [NotNull] public ICloseParenToken CloseParen { get; }
        [CanBeNull] public EffectsSyntax Effects { get; }
        [NotNull] public BlockExpressionSyntax Body { get; }

        protected FunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenToken closeParen,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] BlockExpressionSyntax body)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(body), body);
            Modifiers = modifiers;
            OpenParen = openParen;
            ParameterList = parameterList;
            CloseParen = closeParen;
            Effects = effects;
            Body = body;
        }
    }
}
