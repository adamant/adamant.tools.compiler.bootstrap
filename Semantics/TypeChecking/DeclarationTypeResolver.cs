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

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking
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
        private readonly Diagnostics diagnostics;

        public DeclarationTypeResolver(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public void ResolveTypesInDeclarations([NotNull, ItemNotNull] FixedList<MemberDeclarationSyntax> declarations)
        {
            ResolveSignatureTypesInDeclarations(declarations);
            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            ResolveBodyTypesInDeclarations(declarations);
        }

        private void ResolveSignatureTypesInDeclarations(
             [ItemNotNull] IEnumerable<DeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveSignatureTypesInDeclaration(declaration);
        }

        private void ResolveSignatureTypesInDeclaration(DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax f:
                    ResolveSignatureTypesInFunction(f);
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

        private void ResolveSignatureTypesInFunction(FunctionDeclarationSyntax function)
        {
            function.Type.BeginFulfilling();
            var diagnosticCount = diagnostics.Count;

            // Resolve the declaring type because we need its type for things like `self`
            if (function.DeclaringType != null)
                ResolveSignatureTypesInTypeDeclaration(function.DeclaringType);

            var selfType = ResolveSelfType(function);
            var resolver = new ExpressionTypeResolver(function.File, diagnostics, selfType);

            if (function.GenericParameters != null)
                ResolveTypesInGenericParameters(function.GenericParameters, resolver);

            var parameterTypes = ResolveTypesInParameters(function, resolver);

            var returnType = ResolveReturnType(function, resolver);
            DataType functionType = new FunctionType(parameterTypes, returnType);

            if (function.GenericParameters?.Any() ?? false)
                functionType = new MetaFunctionType(function.GenericParameters.Select(p => p.Type.Fulfilled()), functionType);

            function.Type.Fulfill(functionType);
            if (diagnosticCount != diagnostics.Count) function.Poison();
        }

        private DataType ResolveSelfType(FunctionDeclarationSyntax function)
        {
            var declaringType = function.DeclaringType?.Metatype.Instance;
            if (declaringType == null) return null;

            switch (function)
            {
                case ConstructorDeclarationSyntax constructor:
                    return constructor.SelfParameterType = ((ObjectType)declaringType).ForConstruction();
                case NamedFunctionDeclarationSyntax namedFunction:
                    var selfParameter = namedFunction.Parameters.OfType<SelfParameterSyntax>().SingleOrDefault();
                    if (selfParameter == null) return null; // Static function
                    selfParameter.Type.BeginFulfilling();
                    // TODO deal with structs and ref self
                    var selfType = (ObjectType)declaringType;
                    if (selfParameter.MutableSelf) selfType = selfType.AsMutable();
                    return namedFunction.SelfParameterType = selfParameter.Type.Fulfill(selfType);
                case InitializerDeclarationSyntax _:
                    throw new NotImplementedException();
                default:
                    throw NonExhaustiveMatchException.For(function);
            }
        }

        private static void ResolveTypesInGenericParameters(
            [NotNull, ItemNotNull] FixedList<GenericParameterSyntax> genericParameters,
             ExpressionTypeResolver expressionResolver)
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
             FunctionDeclarationSyntax function,
             ExpressionTypeResolver expressionResolver)
        {
            var types = new List<DataType>();
            foreach (var parameter in function.Parameters)
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                    {
                        parameter.Type.BeginFulfilling();
                        var type =
                            expressionResolver.CheckAndEvaluateTypeExpression(namedParameter
                                .TypeExpression);
                        types.Add(parameter.Type.Fulfill(type));
                    }
                    break;
                    case SelfParameterSyntax _:
                        // Skip, we have already handled the self parameter
                        break;
                    case FieldParameterSyntax fieldParameter:
                        throw new NotImplementedException();
                    default:
                        throw NonExhaustiveMatchException.For(parameter);
                }

            return types.ToFixedList();
        }

        private static DataType ResolveReturnType(
            FunctionDeclarationSyntax function,
            ExpressionTypeResolver expressionResolver)
        {
            function.ReturnType.BeginFulfilling();
            switch (function)
            {
                case NamedFunctionDeclarationSyntax namedFunction:
                    return ResolveReturnType(function, namedFunction.ReturnTypeExpression, expressionResolver);
                case OperatorDeclarationSyntax @operator:
                    return ResolveReturnType(function, @operator.ReturnTypeExpression, expressionResolver);
                case ConstructorDeclarationSyntax _:
                case InitializerDeclarationSyntax _:
                    return function.ReturnType.Fulfill(function.DeclaringType.Metatype.Instance);
                default:
                    throw NonExhaustiveMatchException.For(function);
            }
        }

        private static DataType ResolveReturnType(
            FunctionDeclarationSyntax function,
            ExpressionSyntax returnTypeExpression,
            ExpressionTypeResolver expressionResolver)
        {
            var returnType = returnTypeExpression != null
                ? expressionResolver.CheckAndEvaluateTypeExpression(returnTypeExpression)
                : DataType.Void;

            return function.ReturnType.Fulfill(returnType);
        }

        /// <summary>
        /// If the type has not been resolved, this resolves it. This function
        /// also watches for type cycles and reports an error.
        /// </summary>
        private void ResolveSignatureTypesInTypeDeclaration(TypeDeclarationSyntax declaration)
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
                var genericParameters = declaration.GenericParameters;
                ResolveTypesInGenericParameters(genericParameters, expressionChecker);
                genericParameterTypes = genericParameters.Select(p => p.Type.Fulfilled()).ToFixedList();
            }
            switch (declaration)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var classType = new ObjectType(declaration,
                        classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(classType));
                    break;
                case StructDeclarationSyntax structDeclaration:
                    var structType = new ObjectType(declaration,
                        structDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(structType));
                    break;
                case EnumStructDeclarationSyntax enumStructDeclaration:
                    var enumStructType = new ObjectType(declaration,
                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(enumStructType));
                    break;
                case EnumClassDeclarationSyntax enumStructDeclaration:
                    var enumClassType = new ObjectType(declaration,
                        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(enumClassType));
                    break;
                case TraitDeclarationSyntax declarationSyntax:
                    var type = new ObjectType(declaration,
                        declarationSyntax.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    declaration.Type.Fulfill(new Metatype(type));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveSignatureTypesInField(FieldDeclarationSyntax field)
        {
            var resolver = new ExpressionTypeResolver(field.File, diagnostics);
            field.Type.BeginFulfilling();
            var type = resolver.CheckAndEvaluateTypeExpression(field.TypeExpression);
            field.Type.Fulfill(type);
        }

        private void ResolveBodyTypesInDeclarations(
             [ItemNotNull] IEnumerable<DeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveBodyTypesInDeclaration(declaration);
        }

        private void ResolveBodyTypesInDeclaration(DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax f:
                    ResolveBodyTypesInFunction(f);
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

        private void ResolveBodyTypesInFunction(FunctionDeclarationSyntax function)
        {
            if (function.Body == null) return;

            var diagnosticCount = diagnostics.Count;
            // TODO the return types of constructors and init functions should probably be void for purposes of expressions
            var resolver = new ExpressionTypeResolver(function.File, diagnostics, (Metatype)function.DeclaringType?.Type.Fulfilled(), function.ReturnType.Fulfilled());
            // The body of a function shouldn't itself evaluate to anything.
            // There should be no `=> value` for the block, so the type is `void`.
            resolver.CheckExpressionType(function.Body, DataType.Void);
            if (diagnosticCount != diagnostics.Count) function.Poison();
        }

        private void ResolveBodyTypesInTypeDeclaration(TypeDeclarationSyntax typeDeclaration)
        {
            // No body types, members are already processed
        }

        private void ResolveBodyTypesInField(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Initializer == null) return;

            var resolver = new ExpressionTypeResolver(fieldDeclaration.File, diagnostics);
            resolver.CheckExpressionType(fieldDeclaration.Initializer, fieldDeclaration.Type.Fulfilled());
        }
    }
}
