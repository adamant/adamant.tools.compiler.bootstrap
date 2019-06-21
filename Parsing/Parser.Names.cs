using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public ExpressionSyntax ParseName()
        {
            ExpressionSyntax name = ParseSimpleName();
            while (Tokens.Accept<IDotToken>())
            {
                var simpleName = ParseSimpleName();
                var span = TextSpan.Covering(name.Span, simpleName.Span);
                name = new MemberAccessExpressionSyntax(span, name, AccessOperator.Standard, simpleName);
            }
            return name;
        }

        private NameSyntax ParseSimpleName()
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = new SimpleName(identifier.Value);
            NameSyntax syntax;
            if (Tokens.Accept<IOpenBracketToken>())
            {
                var arguments = ParseArguments();
                var closeBracket = Tokens.Expect<ICloseBracketToken>();
                var span = TextSpan.Covering(identifier.Span, closeBracket);
                syntax = new GenericNameSyntax(span, identifier.Value, arguments);
            }
            else
                syntax = new IdentifierNameSyntax(identifier.Span, name);
            return syntax;
        }
    }
}
