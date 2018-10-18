using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeChecker
    {
        [NotNull] private readonly NameBinder nameBinder;
        [NotNull] private readonly ExpressionAnalysisBuilder expressionAnalysisBuilder;

        public TypeChecker(
            [NotNull] NameBinder nameBinder,
            [NotNull] ExpressionAnalysisBuilder expressionAnalysisBuilder)
        {
            Requires.NotNull(nameof(nameBinder), nameBinder);
            Requires.NotNull(nameof(expressionAnalysisBuilder), expressionAnalysisBuilder);
            this.nameBinder = nameBinder;
            this.expressionAnalysisBuilder = expressionAnalysisBuilder;
        }

        public void CheckTypes(
            [NotNull] IList<DeclarationAnalysis> analyses,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            foreach (var analysis in analyses)
            {
                switch (analysis)
                {
                    case FunctionDeclarationAnalysis f:
                        CheckTypes(f, diagnostics);
                        break;
                    case TypeDeclarationAnalysis t:
                        CheckTypes(t);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(analysis);
                }
            }
        }

        private void CheckTypes(
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var parameterTypes = function.Syntax.Parameters.Nodes().Select(n => n.TypeExpression);
            foreach (var (parameter, type) in function.Semantics.Parameters.Zip(parameterTypes))
            {
                var analysis = expressionAnalysisBuilder.PrepareForAnalysis(function, type);
                CheckTypeExpression(analysis, diagnostics);
                parameter.Type = ResolveType(type, function.Scope);
            }

            var returnType = function.Syntax.ReturnTypeExpression;
            var returnTypeAnalysis = expressionAnalysisBuilder.PrepareForAnalysis(function, returnType);
            CheckTypeExpression(returnTypeAnalysis, diagnostics);
            function.Semantics.ReturnType = ResolveType(returnType, function.Scope);
        }


        private static void CheckTypes([NotNull] TypeDeclarationAnalysis typeDeclaration)
        {
            // TODO
        }

        // Checks the expression is well typed, and that the type of the expression is `type`
        private static void CheckTypeExpression(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(expression);
            if (expression.Type != ObjectType.Type)
                diagnostics.Publish(TypeError.MustBeATypeExpression(expression.File, expression.Syntax.Span));
        }

        private static void CheckTypes([NotNull] ExpressionAnalysis expression)
        {
            switch (expression.Syntax)
            {
                case PrimitiveTypeSyntax _:
                    expression.Type = ObjectType.Type;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression.Syntax);
            }
        }

        [NotNull]
        private static DataType ResolveType(
            [NotNull] ExpressionSyntax typeExpression,
            [NotNull] LexicalScope scope)
        {
            switch (typeExpression)
            {
                case IdentifierNameSyntax identifier:
                    throw new NotImplementedException();
                case PrimitiveTypeSyntax primitive:
                    switch (primitive.Keyword)
                    {
                        case IntKeywordToken _:
                            return ObjectType.Int;
                        case UIntKeywordToken _:
                            return ObjectType.UInt;
                        case ByteKeywordToken _:
                            return ObjectType.Byte;
                        case SizeKeywordToken _:
                            return ObjectType.Size;
                        case VoidKeywordToken _:
                            return ObjectType.Void;
                        case BoolKeywordToken _:
                            return ObjectType.Bool;
                        case StringKeywordToken _:
                            return ObjectType.String;
                        case NeverKeywordToken _:
                            return ObjectType.Never;
                        case TypeKeywordToken _:
                            return ObjectType.Type;
                        default:
                            throw NonExhaustiveMatchException.For(primitive.Keyword);
                    }
                case LifetimeTypeSyntax lifetimeType:
                    return ResolveType(lifetimeType.TypeName, scope);
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}
