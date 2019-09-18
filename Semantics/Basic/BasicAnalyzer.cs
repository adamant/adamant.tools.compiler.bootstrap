using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    /// <summary>
    /// The basic analyzer does name binding, type checking and constant folding.
    /// This class handles declarations and delegates expressions, types etc. to
    /// other classes.
    ///
    /// All basic analysis uses specific terminology to distinguish different
    /// aspects of type checking. (The entry method `Check` is an exception. It
    /// is named to match other analyzers but performs a resolve.)
    ///
    /// Terminology:
    ///
    /// * Resolve - includes type inference and checking
    /// * Check - check something has an expected type
    /// * Infer - infer what type something has
    /// </summary>
    public class BasicAnalyzer
    {
        private readonly Diagnostics diagnostics;

        private BasicAnalyzer(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public static void Check(FixedList<MemberDeclarationSyntax> memberDeclarations, Diagnostics diagnostics)
        {
            var analyzer = new BasicAnalyzer(diagnostics);
            analyzer.ResolveTypesInDeclarations(memberDeclarations);
        }

        private void ResolveTypesInDeclarations(FixedList<MemberDeclarationSyntax> declarations)
        {
            // Process all types first because they may be referenced by functions etc.
            ResolveSignatureTypesInTypeDeclarations(declarations.OfType<TypeDeclarationSyntax>());
            // Now resolve all other types (type declarations will already have types and won't be processed again)
            ResolveSignatureTypesInDeclarations(declarations);
            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            ResolveBodyTypesInDeclarations(declarations);
        }

        private void ResolveSignatureTypesInTypeDeclarations(IEnumerable<TypeDeclarationSyntax> typeDeclarations)
        {
            foreach (var typeDeclaration in typeDeclarations)
                ResolveSignatureTypesInTypeDeclaration(typeDeclaration);
        }

        private void ResolveSignatureTypesInDeclarations(
             IEnumerable<DeclarationSyntax> declarations)
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
            var analyzer = new BasicExpressionAnalyzer(function.File, diagnostics, selfType);

            //if (function.GenericParameters != null)
            //    ResolveTypesInGenericParameters(function.GenericParameters, analyzer);

            var parameterTypes = ResolveTypesInParameters(function, analyzer);

            var returnType = ResolveReturnType(function, analyzer);
            //DataType functionType = new FunctionType(parameterTypes, returnType);

            // TODO handle generic parameters. The function type will just have type parameters in it
            //if (function.GenericParameters?.Any() ?? false)
            //    functionType = new MetaFunctionType(function.GenericParameters.Select(p => p.Type.Fulfilled()), functionType);

            //function.Type.Fulfill(functionType);
            if (diagnosticCount != diagnostics.Count)
                function.MarkErrored();
        }

        private DataType ResolveSelfType(FunctionDeclarationSyntax function)
        {
            var declaringType = function.DeclaringType.DeclaresType;
            if (declaringType == null)
                return null;

            switch (function)
            {
                case ConstructorDeclarationSyntax constructor:
                    return constructor.SelfParameterType = ((UserObjectType)declaringType).ForConstructorSelf();
                case NamedFunctionDeclarationSyntax namedFunction:
                    var selfParameter = namedFunction.Parameters.OfType<SelfParameterSyntax>().SingleOrDefault();
                    if (selfParameter == null)
                        return null; // Static function
                    selfParameter.Type.BeginFulfilling();
                    // TODO deal with structs and ref self
                    var selfType = (UserObjectType)declaringType;
                    if (selfParameter.MutableSelf)
                        selfType = selfType.AsMutable();
                    return namedFunction.SelfParameterType = selfParameter.Type.Fulfill(selfType);
                //case InitializerDeclarationSyntax _:
                //    throw new NotImplementedException();
                default:
                    throw NonExhaustiveMatchException.For(function);
            }
        }

        //private static void ResolveTypesInGenericParameters(
        //    FixedList<GenericParameterSyntax> genericParameters,
        //    BasicExpressionAnalyzer analyzer)
        //{
        //    foreach (var parameter in genericParameters)
        //    {
        //        parameter.Type.BeginFulfilling();
        //        var type = parameter.TypeExpression == null ?
        //            DataType.Type
        //            : analyzer.CheckTypeExpression(parameter.TypeExpression);
        //        parameter.Type.Fulfill(type);
        //    }
        //}

        private FixedList<DataType> ResolveTypesInParameters(
             FunctionDeclarationSyntax function,
             BasicExpressionAnalyzer analyzer)
        {
            var types = new List<DataType>();
            foreach (var parameter in function.Parameters)
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                    {
                        parameter.Type.BeginFulfilling();
                        var type =
                            analyzer.CheckTypeExpression(namedParameter
                                .TypeExpression);
                        types.Add(parameter.Type.Fulfill(type));
                    }
                    break;
                    case SelfParameterSyntax _:
                        // Skip, we have already handled the self parameter
                        break;
                    case FieldParameterSyntax fieldParameter:
                        parameter.Type.BeginFulfilling();
                        var field = function.DeclaringType.Members
                            .OfType<FieldDeclarationSyntax>()
                            .SingleOrDefault(f => f.Name == fieldParameter.FieldName);
                        if (field == null)
                        {
                            parameter.Type.Fulfill(DataType.Unknown);
                            // TODO report an error
                            throw new NotImplementedException();
                        }
                        else
                        {
                            ResolveSignatureTypesInField(field);
                            parameter.Type.Fulfill(field.Type.Fulfilled());
                        }
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(parameter);
                }

            return types.ToFixedList();
        }

        private static DataType ResolveReturnType(
            FunctionDeclarationSyntax function,
            BasicExpressionAnalyzer analyzer)
        {
            function.ReturnType.BeginFulfilling();
            switch (function)
            {
                case NamedFunctionDeclarationSyntax namedFunction:
                    return ResolveReturnType(function, namedFunction.ReturnTypeExpression, analyzer);
                //case OperatorDeclarationSyntax @operator:
                //    return ResolveReturnType(function, @operator.ReturnTypeExpression, analyzer);
                case ConstructorDeclarationSyntax _:
                    //case InitializerDeclarationSyntax _:
                    return function.ReturnType.Fulfill(function.DeclaringType.DeclaresType);
                default:
                    throw NonExhaustiveMatchException.For(function);
            }
        }

        private static DataType ResolveReturnType(
            FunctionDeclarationSyntax function,
            ExpressionSyntax returnTypeExpression,
            BasicExpressionAnalyzer analyzer)
        {
            var returnType = returnTypeExpression != null
                ? analyzer.CheckTypeExpression(returnTypeExpression)
                : DataType.Void;

            // If we are returning ownership, then they can make it mutable
            if (returnType is UserObjectType objectType && objectType.IsOwned)
                returnType = objectType.AsImplicitlyUpgradable();
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

            var expressionChecker = new BasicExpressionAnalyzer(declaration.File, diagnostics);

            FixedList<DataType> genericParameterTypes = null;
            //if (declaration.GenericParameters != null)
            //{
            //    var genericParameters = declaration.GenericParameters;
            //    ResolveTypesInGenericParameters(genericParameters, expressionChecker);
            //    genericParameterTypes = genericParameters.Select(p => p.Type.Fulfilled()).ToFixedList();
            //}
            switch (declaration)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var classType = UserObjectType.Declaration(declaration,
                        classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                        genericParameterTypes);
                    //declaration.Type.Fulfill(new Metatype(classType));
                    classDeclaration.CreateDefaultConstructor();
                    break;
                //case StructDeclarationSyntax structDeclaration:
                //    var structType = UserObjectType.Declaration(declaration,
                //        structDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                //        genericParameterTypes);
                //    declaration.Type.Fulfill(new Metatype(structType));
                //    break;
                //case EnumStructDeclarationSyntax enumStructDeclaration:
                //    var enumStructType = UserObjectType.Declaration(declaration,
                //        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                //        genericParameterTypes);
                //    declaration.Type.Fulfill(new Metatype(enumStructType));
                //    break;
                //case EnumClassDeclarationSyntax enumStructDeclaration:
                //    var enumClassType = UserObjectType.Declaration(declaration,
                //        enumStructDeclaration.Modifiers.Any(m => m is IMutableKeywordToken),
                //        genericParameterTypes);
                //    declaration.Type.Fulfill(new Metatype(enumClassType));
                //    break;
                //case TraitDeclarationSyntax declarationSyntax:
                //    var type = UserObjectType.Declaration(declaration,
                //        declarationSyntax.Modifiers.Any(m => m is IMutableKeywordToken),
                //        genericParameterTypes);
                //    declaration.Type.Fulfill(new Metatype(type));
                //    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void ResolveSignatureTypesInField(FieldDeclarationSyntax field)
        {
            switch (field.Type.State)
            {
                case PromiseState.InProgress:
                    diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan, field.Name));
                    return;
                case PromiseState.Fulfilled:
                    return;   // We have already resolved it
                case PromiseState.Pending:
                    // we need to compute it
                    break;
            }
            var resolver = new BasicExpressionAnalyzer(field.File, diagnostics);
            field.Type.BeginFulfilling();
            var type = resolver.CheckTypeExpression(field.TypeExpression);
            field.Type.Fulfill(type);
        }

        private void ResolveBodyTypesInDeclarations(
             IEnumerable<DeclarationSyntax> declarations)
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
            if (function.Body == null)
                return;

            var diagnosticCount = diagnostics.Count;
            // TODO the return types of constructors and init functions should probably be void for purposes of expressions
            var resolver = new BasicExpressionAnalyzer(function.File, diagnostics, function.SelfParameterType, function.ReturnType.Fulfilled());
            // The body of a function shouldn't itself evaluate to anything.
            // There should be no `=> value` for the block, so the type is `void`.
            resolver.CheckExpressionType(function.Body, DataType.Void);
            if (diagnosticCount != diagnostics.Count)
                function.MarkErrored();
        }

        private void ResolveBodyTypesInTypeDeclaration(TypeDeclarationSyntax _)
        {
            // No body types, members are already processed
        }

        private void ResolveBodyTypesInField(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Initializer == null)
                return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new BasicExpressionAnalyzer(fieldDeclaration.File, diagnostics);
            resolver.CheckExpressionType(fieldDeclaration.Initializer, fieldDeclaration.Type.Fulfilled());
            if (diagnosticCount != diagnostics.Count)
                fieldDeclaration.MarkErrored();
        }
    }
}
