using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    /// <summary>
    /// Terminology:
    ///
    /// * Resolve: includes type inference, checking and evaluation, whichever is appropriate
    /// * Check: there is a type something is expected to be compatible with, check that it is
    /// * Infer:
    /// </summary>
    public class DeclarationTypeChecker
    {
        [NotNull] private readonly Diagnostics diagnostics;

        public DeclarationTypeChecker([NotNull] Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public void ResolveTypesInDeclarations([NotNull, ItemNotNull] FixedList<INamespacedDeclarationSyntax> declarations)
        {
            ResolveSignatureTypesInDeclarations(declarations);
            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            ResolveBodyTypesInDeclarations(declarations);
        }

        private void ResolveSignatureTypesInDeclarations([NotNull, ItemNotNull] FixedList<INamespacedDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveSignatureTypesInDeclaration(declaration);
        }

        private void ResolveSignatureTypesInDeclaration([NotNull] INamespacedDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax f:
                    ResolveSignatureTypesInFunction(f);
                    break;
                case TypeDeclarationSyntax t:
                    ResolveSignatureTypesInTypeDeclaration(t);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveSignatureTypesInFunction([NotNull] FunctionDeclarationSyntax function)
        {
            function.Type.BeginFulfilling();

            var expressionChecker = new ExpressionTypeChecker(function.File, diagnostics);

            if (function.GenericParameters != null)
                ResolveTypesInGenericParameters(function.GenericParameters, expressionChecker);

            var parameterTypes = ResolveTypesInParameters(function, expressionChecker);

            var returnType = ResolveReturnType(function, expressionChecker);
            DataType functionType = new FunctionType(parameterTypes, returnType);

            if (function.GenericParameters?.Any() ?? false)
                functionType = new MetaFunctionType(function.GenericParameters.NotNull().Select(p => p.Type.Fulfilled()), functionType);

            function.Type.Fulfill(functionType);
        }

        private static void ResolveTypesInGenericParameters(
            [NotNull, ItemNotNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] ExpressionTypeChecker expressionChecker)
        {
            foreach (var parameter in genericParameters)
            {
                parameter.Type.BeginFulfilling();
                var type = parameter.TypeExpression == null ?
                    ObjectType.Type
                    : expressionChecker.CheckAndEvaluateTypeExpression(parameter.TypeExpression);
                parameter.Type.Fulfill(type);
            }
        }

        [NotNull, ItemNotNull]
        private FixedList<DataType> ResolveTypesInParameters(
            [NotNull] FunctionDeclarationSyntax function,
            [NotNull] ExpressionTypeChecker expressionChecker)
        {
            var types = new List<DataType>();
            foreach (var parameter in function.Parameters)
            {
                parameter.Type.BeginFulfilling();
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                        var type = expressionChecker.CheckAndEvaluateTypeExpression(namedParameter.TypeExpression);
                        types.Add(parameter.Type.Fulfill(type));
                        break;
                    case SelfParameterSyntax selfParameter:
                        diagnostics.Add(TypeError.NotImplemented(function.File,
                            parameter.Span, "Self parameters not implemented"));
                        types.Add(parameter.Type.Fulfill(DataType.Unknown));
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(parameter);
                }
            }

            return types.ToFixedList();
        }

        [NotNull]
        private static DataType ResolveReturnType(
            [NotNull] FunctionDeclarationSyntax function,
            [NotNull] ExpressionTypeChecker expressionChecker)
        {
            function.ReturnType.BeginFulfilling();
            switch (function)
            {
                case NamedFunctionDeclarationSyntax namedFunction:
                    var returnType = namedFunction.ReturnTypeExpression != null
                        ? expressionChecker.CheckAndEvaluateTypeExpression(namedFunction.ReturnTypeExpression)
                        : ObjectType.Void;
                    return function.ReturnType.Fulfill(returnType);
                default:
                    throw NonExhaustiveMatchException.For(function);
            }
        }

        /// <summary>
        /// If the type has not been checked, this checks it and returns it.
        /// Also watches for type cycles
        /// </summary>
        private void ResolveSignatureTypesInTypeDeclaration([NotNull] TypeDeclarationSyntax declaration)
        {
            switch (declaration.Type.State)
            {
                case PromiseState.InProgress:
                    diagnostics.Add(TypeError.CircularDefinition(declaration.File, declaration.NameSpan, declaration.Name));
                    return;
                case PromiseState.Fulfilled:
                    return;   // We have already resolved it
                case PromiseState.Pending:
                    // we need to compute it
                    break;
            }

            declaration.Type.BeginFulfilling();

            var expressionChecker = new ExpressionTypeChecker(declaration.File, diagnostics);

            FixedList<DataType> genericParameterTypes = null;
            if (declaration.GenericParameters != null)
            {
                var genericParameters = declaration.GenericParameters.NotNull();
                ResolveTypesInGenericParameters(genericParameters, expressionChecker);
                genericParameterTypes = genericParameters.Select(p => p.Type.Fulfilled()).ToFixedList();
            }
            switch (declaration)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var classType = new ObjectType(declaration.Name, true,
                        classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(classType));
                    break;
                case StructDeclarationSyntax structDeclaration:
                    var structType = new ObjectType(declaration.Name, false,
                        structDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(structType));
                    break;
                case EnumStructDeclarationSyntax enumStructDeclaration:
                    var enumStructType = new ObjectType(declaration.Name, false,
                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(enumStructType));
                    break;
                case EnumClassDeclarationSyntax enumStructDeclaration:
                    var enumClassType = new ObjectType(declaration.Name, true,
                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(enumClassType));
                    break;
                case TraitDeclarationSyntax declarationSyntax:
                    var type = new ObjectType(declaration.Name, true,
                        declarationSyntax.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(type));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveBodyTypesInDeclarations([NotNull, ItemNotNull] FixedList<INamespacedDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveBodyTypesInDeclaration(declaration);
        }

        private void ResolveBodyTypesInDeclaration(INamespacedDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax f:
                    ResolveBodyTypesInFunction(f);
                    break;
                case TypeDeclarationSyntax t:
                    ResolveBodyTypesInTypeDeclaration(t);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveBodyTypesInFunction([NotNull] FunctionDeclarationSyntax function)
        {
            if (function.Body == null) return;

            var expressionChecker = new ExpressionTypeChecker(function.File, diagnostics, function.ReturnType.Fulfilled());
            // The body of a function shouldn't itself evaluate to anything.
            // There should be no `=> value` for the block, so the type is `void`.
            expressionChecker.CheckExpressionType(function.Body, ObjectType.Void);
        }

        private static void ResolveBodyTypesInTypeDeclaration(TypeDeclarationSyntax typeDeclaration)
        {
            throw new NotImplementedException();
        }
    }
}
