using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

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
             IEnumerable<MemberDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveSignatureTypesInDeclaration(declaration);
        }

        private void ResolveSignatureTypesInDeclaration(MemberDeclarationSyntax declaration)
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
                    throw ExhaustiveMatch.Failed(declaration);
            }
        }

        private void ResolveSignatureTypesInFunction(FunctionDeclarationSyntax function)
        {
            var diagnosticCount = diagnostics.Count;

            // Resolve the declaring type because we need its type for things like `self`
            if (function.DeclaringType != null)
                ResolveSignatureTypesInTypeDeclaration(function.DeclaringType);

            var selfType = ResolveSelfType(function);
            var analyzer = new BasicStatementAnalyzer(function.File, diagnostics, selfType);

            ResolveTypesInParameters(function, analyzer);

            ResolveReturnType(function, analyzer);

            if (diagnosticCount != diagnostics.Count)
                function.MarkErrored();
        }

        private static DataType? ResolveSelfType(FunctionDeclarationSyntax function)
        {
            var declaringType = function.DeclaringType?.DeclaresType.Fulfilled();
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
                    var selfType = (UserObjectType)declaringType;
                    if (selfParameter.MutableSelf)
                        selfType = selfType.AsMutable();
                    return namedFunction.SelfParameterType = selfParameter.Type.Fulfill(selfType);
                default:
                    throw NonExhaustiveMatchException.For(function);
            }
        }

        private void ResolveTypesInParameters(
            FunctionDeclarationSyntax function,
            BasicStatementAnalyzer analyzer)
        {
            var types = new List<DataType>();
            foreach (var parameter in function.Parameters)
                switch (parameter)
                {
                    case NamedParameterSyntax namedParameter:
                    {
                        parameter.Type.BeginFulfilling();
                        var type = analyzer
                            .EvaluateType(namedParameter.TypeSyntax);
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
                        throw ExhaustiveMatch.Failed(parameter);
                }

            types.ToFixedList();
        }

        private static void ResolveReturnType(
            FunctionDeclarationSyntax function,
            BasicStatementAnalyzer analyzer)
        {
            function.ReturnType.BeginFulfilling();
            switch (function)
            {
                case NamedFunctionDeclarationSyntax namedFunction:
                    ResolveReturnType(function, namedFunction.ReturnTypeSyntax, analyzer);
                    return;
                case ConstructorDeclarationSyntax _:
                    function.ReturnType.Fulfill(function.DeclaringType.DeclaresType.Fulfilled());
                    return;
                default:
                    throw ExhaustiveMatch.Failed(function);
            }
        }

        private static void ResolveReturnType(
            FunctionDeclarationSyntax function,
            TypeSyntax returnTypeSyntax,
            BasicStatementAnalyzer analyzer)
        {
            var returnType = returnTypeSyntax != null
                ? analyzer.EvaluateType(returnTypeSyntax)
                : DataType.Void;

            // If we are returning ownership, then they can make it mutable
            if (returnType is UserObjectType objectType && objectType.IsOwned)
                returnType = objectType.AsImplicitlyUpgradable();
            function.ReturnType.Fulfill(returnType);
        }

        /// <summary>
        /// If the type has not been resolved, this resolves it. This function
        /// also watches for type cycles and reports an error.
        /// </summary>
        private void ResolveSignatureTypesInTypeDeclaration(TypeDeclarationSyntax declaration)
        {
            switch (declaration.DeclaresType.State)
            {
                case PromiseState.InProgress:
                    diagnostics.Add(TypeError.CircularDefinition(declaration.File, declaration.NameSpan, declaration.Name));
                    return;
                case PromiseState.Fulfilled:
                    return;   // We have already resolved it
                case PromiseState.Pending:
                    // we need to compute it
                    break;
                default:
                    throw ExhaustiveMatch.Failed(declaration.DeclaresType.State);
            }

            declaration.DeclaresType.BeginFulfilling();

            switch (declaration)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var classType = UserObjectType.Declaration(declaration,
                        classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken));
                    declaration.DeclaresType.Fulfill(classType);
                    classDeclaration.CreateDefaultConstructor();
                    break;
                default:
                    throw ExhaustiveMatch.Failed(declaration);
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
                default:
                    throw ExhaustiveMatch.Failed(field.Type.State);
            }
            var resolver = new BasicStatementAnalyzer(field.File, diagnostics);
            field.Type.BeginFulfilling();
            var type = resolver.EvaluateType(field.TypeSyntax);
            field.Type.Fulfill(type);
        }

        private void ResolveBodyTypesInDeclarations(
             IEnumerable<MemberDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveBodyTypesInDeclaration(declaration);
        }

        private void ResolveBodyTypesInDeclaration(MemberDeclarationSyntax declaration)
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
                    throw ExhaustiveMatch.Failed(declaration);
            }
        }

        private void ResolveBodyTypesInFunction(FunctionDeclarationSyntax function)
        {
            if (function.Body == null)
                return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new BasicStatementAnalyzer(function.File, diagnostics, function.SelfParameterType, function.ReturnType.Fulfilled());
            foreach (var statement in function.Body)
                resolver.ResolveTypesInStatement(statement);
            if (diagnosticCount != diagnostics.Count)
                function.MarkErrored();
        }

        private static void ResolveBodyTypesInTypeDeclaration(TypeDeclarationSyntax _)
        {
            // No body types, members are already processed
        }

        private void ResolveBodyTypesInField(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Initializer == null)
                return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new BasicStatementAnalyzer(fieldDeclaration.File, diagnostics);
            resolver.CheckExpressionType(ref fieldDeclaration.Initializer, fieldDeclaration.Type.Fulfilled());
            if (diagnosticCount != diagnostics.Count)
                fieldDeclaration.MarkErrored();
        }
    }
}
