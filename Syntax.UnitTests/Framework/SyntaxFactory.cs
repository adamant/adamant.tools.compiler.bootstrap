using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework
{
    public class SyntaxFactory
    {
        public SimpleToken SimpleToken(TokenKind kind)
        {
            return new SimpleToken(kind, new TextSpan(0, 0));
        }

        public IdentifierToken Identifier(string name)
        {
            return new IdentifierToken(TokenKind.Identifier, new TextSpan(0, 0), name);
        }

        public FunctionDeclarationSyntax Function(
            SimpleToken accessModifier,
            string name,
            TypeSyntax returnType,
            params StatementSyntax[] statements)
        {
            return Function(accessModifier, name, Parameters(), returnType, statements);
        }

        public SeparatedListSyntax<ParameterSyntax> Parameters(params ParameterSyntax[] parameters)
        {
            return new SeparatedListSyntax<ParameterSyntax>(
                parameters.Cast<ISyntaxNodeOrToken>()
                    .SelectMany(p => new[] { (Token)SimpleToken(TokenKind.Comma), p })
                    .Skip(1)); // Skip first comma
        }

        public FunctionDeclarationSyntax Function(
            SimpleToken accessModifier,
            string name,
            SeparatedListSyntax<ParameterSyntax> parameters,
            TypeSyntax returnType,
            params StatementSyntax[] statements)
        {
            return new FunctionDeclarationSyntax(
                accessModifier,
                SimpleToken(TokenKind.FunctionKeyword),
                Identifier(name),
                SimpleToken(TokenKind.OpenParen),
                parameters,
                SimpleToken(TokenKind.CloseParen),
                SimpleToken(TokenKind.RightArrow),
                returnType,
                Block(statements));
        }

        public BlockStatementSyntax Block(params StatementSyntax[] statements)
        {
            return new BlockStatementSyntax(SimpleToken(TokenKind.OpenBrace), statements.ToSyntaxList(), SimpleToken(TokenKind.CloseBrace));
        }

        public SimpleToken Public()
        {
            return SimpleToken(TokenKind.PublicKeyword);
        }

        public UsingDirectiveSyntax[] Usings(params UsingDirectiveSyntax[] usingDirectives)
        {
            return usingDirectives;
        }

        public CompilationUnitSyntax CompilationUnit(
            CompilationUnitNamespaceSyntax @namespace = null,
            SyntaxList<UsingDirectiveSyntax> usingDirectives = null,
            params DeclarationSyntax[] declarations)
        {
            return new CompilationUnitSyntax(
                null,
                usingDirectives ?? SyntaxList<UsingDirectiveSyntax>.Empty,
                declarations.ToSyntaxList(),
                new EndOfFileToken());
        }

        public CompilationUnitSyntax CompilationUnit(params DeclarationSyntax[] declarations)
        {
            return CompilationUnit(null, null, declarations);
        }

        public TypeSyntax Void()
        {
            return new PrimitiveTypeSyntax(SimpleToken(TokenKind.VoidKeyword));
        }
    }
}
