using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public abstract IIdentifierTokenPlace Name { get; }
        [NotNull] public IOpenParenTokenPlace OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ParameterSyntax> ParameterList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ParameterSyntax> Parameters => ParameterList.Nodes();
        [NotNull] public ICloseParenTokenPlace CloseParen { get; }
        [CanBeNull] public EffectsSyntax Effects { get; }
        [NotNull] public SyntaxList<FunctionContractSyntax> Contracts { get; }
        [CanBeNull] public BlockSyntax Body { get; }
        [CanBeNull] public ISemicolonTokenPlace Semicolon { get; }

        protected FunctionDeclarationSyntax(
            TextSpan signatureSpan,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ParameterSyntax> parameterList,
            [NotNull] ICloseParenTokenPlace closeParen,
            [CanBeNull] EffectsSyntax effects,
            [NotNull] SyntaxList<FunctionContractSyntax> contracts,
            [CanBeNull] BlockSyntax body,
            [CanBeNull] ISemicolonTokenPlace semicolon)
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
