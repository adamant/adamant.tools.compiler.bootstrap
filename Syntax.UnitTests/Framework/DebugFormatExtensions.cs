using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Framework
{
    public static class DebugFormatExtensions
    {
        public static string DebugFormat(this IEnumerable<Diagnostic> diagnostics)
        {
            return string.Join(", ",
                diagnostics.Select(d =>
                    $"{d.ErrorCode}@{d.StartPosition.Line}:{d.StartPosition.Column}"));
        }

        public static string DebugFormat(this IEnumerable<PsuedoToken> tokens)
        {
            return string.Join(", ", tokens);
        }

        public static string DebugFormat(this SyntaxNode node)
        {
            var builder = new StringBuilder();
            node.DebugFormat(builder);
            return builder.ToString();
        }

        public static void DebugFormat(this SyntaxNode node, StringBuilder builder)
        {
            switch (node)
            {
                case null:
                    break;
                case CompilationUnitSyntax cu:
                    cu.Namespace.DebugFormat(builder);
                    foreach (var usingDirective in cu.UsingDirectives)
                        usingDirective.DebugFormat(builder);
                    foreach (var declaration in cu.Declarations)
                        declaration.DebugFormat(builder);
                    break;
                case FunctionDeclarationSyntax function:
                    function.AccessModifier.DebugFormat(builder);
                    builder.Append(" fn ");
                    function.Name.DebugFormat(builder);
                    builder.Append("(");
                    function.Parameters.DebugFormat(builder);
                    builder.Append(") -> ");
                    function.ReturnTypeExpression.DebugFormat(builder);
                    function.Body.DebugFormat(builder);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(node);
            }
        }

        public static void DebugFormat(this SimpleToken token, StringBuilder builder)
        {
            throw new NotImplementedException();
        }

        public static void DebugFormat(this IdentifierToken token, StringBuilder builder)
        {
            builder.Append(token.Value);
        }
    }
}
