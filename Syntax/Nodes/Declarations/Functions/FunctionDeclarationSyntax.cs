using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
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
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public abstract IIdentifierToken Name { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ParameterSyntax> ParameterList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ParameterSyntax> Parameters => ParameterList.Nodes();
        [NotNull] public ICloseParenToken CloseParen { get; }
        [CanBeNull] public EffectsSyntax Effects { get; }
        [NotNull] public SyntaxList<FunctionContractSyntax> Contracts { get; }
        [CanBeNull] public BlockSyntax Body { get; }
        [CanBeNull] public ISemicolonToken Semicolon { get; }

        protected FunctionDeclarationSyntax(
            TextSpan signatureSpan,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenToken closeParen,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [CanBeNull] BlockSyntax body,
            [CanBeNull] ISemicolonToken semicolon)
            : base(signatureSpan)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(parameterList), parameterList);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(contracts), contracts);
            Modifiers = modifiers;
            OpenParen = openParen;
            ParameterList = parameterList;
            CloseParen = closeParen;
            Effects = effects;
            Contracts = contracts;
            Body = body;
            Semicolon = semicolon;
        }
    }
}
