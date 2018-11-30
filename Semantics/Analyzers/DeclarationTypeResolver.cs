using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
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
    public class DeclarationTypeResolver
    {
        [NotNull] private readonly Diagnostics diagnostics;

        public DeclarationTypeResolver([NotNull] Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public void ResolveTypesInDeclarations([NotNull, ItemNotNull] FixedList<INamespacedDeclarationSyntax> declarations)
        {
            ResolveSignatureTypesInDeclarations(declarations.Select(AsDeclarationSyntax));
            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            ResolveBodyTypesInDeclarations(declarations.Select(AsDeclarationSyntax));
        }

        private void ResolveSignatureTypesInDeclarations(
            [NotNull] [ItemNotNull] IEnumerable<DeclarationSyntax> declarations,
            [CanBeNull] TypeDeclarationSyntax declaringType = null)
        {
            foreach (var declaration in declarations)
                ResolveSignatureTypesInDeclaration(declaration, declaringType);
        }

        private void ResolveSignatureTypesInDeclaration(
            [NotNull] DeclarationSyntax declaration,
            [CanBeNull] TypeDeclarationSyntax declaringType)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax f:
                    ResolveSignatureTypesInFunction(f, declaringType);
                    break;
                case TypeDeclarationSyntax t:
                    ResolveSignatureTypesInTypeDeclaration(t);
                    break;
                case FieldDeclarationSyntax f:
                    ResolveSignatureTypesInField(f);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveSignatureTypesInFunction(
            [NotNull] FunctionDeclarationSyntax function,
            [CanBeNull] TypeDeclarationSyntax declaringType)
        {
            function.Type.BeginFulfilling();
            var diagnosticCount = diagnostics.Count;

            var resolver = new ExpressionTypeResolver(function.File, diagnostics, declaringType: declaringType?.Type.Fulfilled());

            if (function.GenericParameters != null)
                ResolveTypesInGenericParameters(function.GenericParameters, resolver);

            var parameterTypes = ResolveTypesInParameters(function, resolver, declaringType);

            var returnType = ResolveReturnType(function, resolver, declaringType);
            DataType functionType = new FunctionType(parameterTypes, returnType);

            if (function.GenericParameters?.Any() ?? false)
                functionType = new MetaFunctionType(function.GenericParameters.NotNull().Select(p => p.Type.Fulfilled()), functionType);

            function.Type.Fulfill(functionType);
            if (diagnosticCount != diagnostics.Count) function.Poison();
        }

        private static void ResolveTypesInGenericParameters(
            [NotNull, ItemNotNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] ExpressionTypeResolver expressionResolver)
        {
            foreach (var parameter in genericParameters)
            {
                parameter.Type.BeginFulfilling();
                var type = parameter.TypeExpression == null ?
                    DataType.Type
                    : expressionResolver.CheckAndEvaluateTypeExpression(parameter.TypeExpression);
                parameter.Type.Fulfill(type);
            }
        }

        [NotNull, ItemNotNull]
        private FixedList<DataType> ResolveTypesInParameters(
            [NotNull] FunctionDeclarationSyntax function,
            [NotNull] ExpressionTypeResolver expressionResolver,
            [CanBeNull] TypeDeclarationSyntax declaringType)
        {
            var types = new List<DataType>();
            foreach (var parameter in function.Parameters)
            {
                parameter.Type.BeginFulfilling();
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                    {
                        var type = expressionResolver
                            .CheckAndEvaluateTypeExpression(namedParameter.TypeExpression);
                        types.Add(parameter.Type.Fulfill(type));
                    }
                    break;
                    case SelfParameterSyntax _:
                    {
                        var type = declaringType?.Type.Fulfilled().Instance ?? DataType.Unknown;
                        types.Add(parameter.Type.Fulfill(type));
                    }
                    break;
                    case FieldParameterSyntax fieldParameter:
                        throw new NotImplementedException();
                    default:
                        throw NonExhaustiveMatchException.For(parameter);
                }
            }

            return types.ToFixedList();
        }

        [NotNull]
        private static DataType ResolveReturnType(
            [NotNull] FunctionDeclarationSyntax function,
            [NotNull] ExpressionTypeResolver expressionResolver,
            [CanBeNull] TypeDeclarationSyntax declaringType)
        {
            function.ReturnType.BeginFulfilling();
            switch (function)
            {
                case NamedFunctionDeclarationSyntax namedFunction:
                {
                    var returnType = namedFunction.ReturnTypeExpression != null
                        ? expressionResolver.CheckAndEvaluateTypeExpression(namedFunction
                            .ReturnTypeExpression)
                        : DataType.Void;
                    return function.ReturnType.Fulfill(returnType);
                }
                case OperatorDeclarationSyntax operatorDeclaration:
                {
                    var returnType = operatorDeclaration.ReturnTypeExpression != null
                        ? expressionResolver.CheckAndEvaluateTypeExpression(operatorDeclaration
                            .ReturnTypeExpression)
                        : DataType.Void;
                    return function.ReturnType.Fulfill(returnType);
                }
                case ConstructorDeclarationSyntax _:
                case InitializerDeclarationSyntax _:
                {
                    var returnType = declaringType.NotNull().Type.Fulfilled().Instance;
                    return function.ReturnType.Fulfill(returnType);
                }
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

            var expressionChecker = new ExpressionTypeResolver(declaration.File, diagnostics);

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
                    var classType = new ObjectType(declaration, true,
                        classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(classType));
                    break;
                case StructDeclarationSyntax structDeclaration:
                    var structType = new ObjectType(declaration, false,
                        structDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(structType));
                    break;
                case EnumStructDeclarationSyntax enumStructDeclaration:
                    var enumStructType = new ObjectType(declaration, false,
                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(enumStructType));
                    break;
                case EnumClassDeclarationSyntax enumStructDeclaration:
                    var enumClassType = new ObjectType(declaration, true,
                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(enumClassType));
                    break;
                case TraitDeclarationSyntax declarationSyntax:
                    var type = new ObjectType(declaration, true,
                        declarationSyntax.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(type));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
            ResolveSignatureTypesInDeclarations(declaration.Members.Select(m => m.AsDeclarationSyntax), declaration);
        }

        private void ResolveSignatureTypesInField([NotNull] FieldDeclarationSyntax field)
        {
            var resolver = new ExpressionTypeResolver(field.File, diagnostics);
            field.Type.BeginFulfilling();
            var type = resolver.CheckAndEvaluateTypeExpression(field.TypeExpression);
            field.Type.Fulfill(type);
        }

        private void ResolveBodyTypesInDeclarations(
            [NotNull, ItemNotNull] IEnumerable<DeclarationSyntax> declarations,
            [CanBeNull] TypeDeclarationSyntax declaringType = null)
        {
            foreach (var declaration in declarations)
                ResolveBodyTypesInDeclaration(declaration, declaringType);
        }

        private void ResolveBodyTypesInDeclaration(
            DeclarationSyntax declaration,
            [CanBeNull] TypeDeclarationSyntax declaringType)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax f:
                    ResolveBodyTypesInFunction(f, declaringType);
                    break;
                case TypeDeclarationSyntax t:
                    ResolveBodyTypesInTypeDeclaration(t);
                    break;
                case FieldDeclarationSyntax f:
                    ResolveBodyTypesInField(f);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveBodyTypesInFunction(
            [NotNull] FunctionDeclarationSyntax function,
            [CanBeNull] TypeDeclarationSyntax declaringType)
        {
            if (function.Body == null) return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new ExpressionTypeResolver(function.File, diagnostics, declaringType: declaringType?.Type.Fulfilled(), returnType: function.ReturnType.Fulfilled());
            // The body of a function shouldn't itself evaluate to anything.
            // There should be no `=> value` for the block, so the type is `void`.
            resolver.CheckExpressionType(function.Body, DataType.Void);
            if (diagnosticCount != diagnostics.Count) function.Poison();
        }

        private void ResolveBodyTypesInTypeDeclaration([NotNull] TypeDeclarationSyntax typeDeclaration)
        {
            ResolveBodyTypesInDeclarations(typeDeclaration.Members.Select(AsDeclarationSyntax), typeDeclaration);
        }

        private void ResolveBodyTypesInField([NotNull] FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Initializer == null) return;

            var resolver = new ExpressionTypeResolver(fieldDeclaration.File, diagnostics);
            resolver.CheckExpressionType(fieldDeclaration.Initializer, fieldDeclaration.Type.Fulfilled());
        }

        private static DeclarationSyntax AsDeclarationSyntax([NotNull] IDeclarationSyntax m)
        {
            return m.AsDeclarationSyntax;
        }
    }
}
