using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Effects;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions.Parameters;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions
{
    public class DestructorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public DeleteKeywordToken DeleteKeyword { get; }
        public override IIdentifierToken Name => throw new System.NotImplementedException();

        public DestructorFunctionDeclarationSyntax(
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] DeleteKeywordToken deleteKeyword,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameters,
            [NotNull] ICloseParenToken closeParen,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<ContractSyntax> contracts,
            [NotNull] BlockSyntax body)
            : base(deleteKeyword.Span, modifiers, openParen, parameters, closeParen,
                effects, contracts, body, null)
        {
            Requires.NotNull(nameof(deleteKeyword), deleteKeyword);
            Requires.NotNull(nameof(body), body);
            DeleteKeyword = deleteKeyword;
        }
    }
}
