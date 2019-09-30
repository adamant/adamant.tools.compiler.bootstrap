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
            ResolveSignatureTypesInClassDeclarations(declarations.OfType<ClassDeclarationSyntax>());
            // Now resolve all other types (type declarations will already have types and won't be processed again)
            ResolveSignatureTypesInDeclarations(declarations);
            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            ResolveBodyTypesInDeclarations(declarations);
        }

        private void ResolveSignatureTypesInClassDeclarations(IEnumerable<ClassDeclarationSyntax> typeDeclarations)
        {
            foreach (var typeDeclaration in typeDeclarations)
                ResolveSignatureTypesInClassDeclaration(typeDeclaration);
        }

        private void ResolveSignatureTypesInDeclarations(
             IEnumerable<MemberDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveSignatureTypesInDeclaration(declaration);
        }

        private void ResolveSignatureTypesInDeclaration(IMemberDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case IFunctionDeclarationSyntax f:
                    ResolveSignatureTypesInFunction(f);
                    break;
                case IConstructorDeclarationSyntax c:
                    ResolveSignatureTypesInConstructor(c);
                    break;
                case IFieldDeclarationSyntax f:
                    ResolveSignatureTypesInField((FieldDeclarationSyntax)f);
                    break;
                default:
                    throw ExhaustiveMatch.Failed(declaration);
            }
        }

        private void ResolveSignatureTypesInConstructor(IConstructorDeclarationSyntax constructor)
        {
            var diagnosticCount = diagnostics.Count;

            // Resolve the declaring type because we need its type for things like `self`
            if (constructor.DeclaringType != null)
                ResolveSignatureTypesInClassDeclaration(constructor.DeclaringType);

            var selfType = constructor.DeclaringType?.DeclaresType.Fulfilled();
            constructor.SelfParameterType = ((UserObjectType)selfType).ForConstructorSelf();
            var analyzer = new BasicStatementAnalyzer(constructor.File, diagnostics, selfType);

            ResolveTypesInParameters(analyzer, constructor.Parameters, constructor.DeclaringType);

            if (diagnosticCount != diagnostics.Count)
                constructor.MarkErrored();
        }


        private void ResolveSignatureTypesInFunction(IFunctionDeclarationSyntax function)
        {
            var diagnosticCount = diagnostics.Count;

            // Resolve the declaring type because we need its type for things like `self`
            if (function.DeclaringType != null)
                ResolveSignatureTypesInClassDeclaration(function.DeclaringType);

            var selfType = ResolveSelfType(function);
            var analyzer = new BasicStatementAnalyzer(function.File, diagnostics, selfType);

            ResolveTypesInParameters(analyzer, function.Parameters, function.DeclaringType);

            ResolveReturnType(function, analyzer);

            if (diagnosticCount != diagnostics.Count)
                function.MarkErrored();
        }

        private static DataType? ResolveSelfType(IFunctionDeclarationSyntax function)
        {
            var declaringType = function.DeclaringType?.DeclaresType.Fulfilled();
            if (declaringType == null)
                return null;

            switch (function)
            {
                default:
                    throw ExhaustiveMatch.Failed(function);
                case INamedFunctionDeclarationSyntax namedFunction:
                    var selfParameter = namedFunction.Parameters.OfType<SelfParameterSyntax>().SingleOrDefault();
                    if (selfParameter == null)
                        return null; // Static function
                    selfParameter.Type.BeginFulfilling();
                    var selfType = (UserObjectType)declaringType;
                    if (selfParameter.MutableSelf)
                        selfType = selfType.AsMutable();
                    return namedFunction.SelfParameterType = selfParameter.Type.Fulfill(selfType);
            }
        }

        private void ResolveTypesInParameters(
            BasicStatementAnalyzer analyzer,
            FixedList<ParameterSyntax> parameters,
            ClassDeclarationSyntax declaringType)
        {
            var types = new List<DataType>();
            foreach (var parameter in parameters)
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
                        var field = declaringType.Members
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
            IFunctionDeclarationSyntax function,
            BasicStatementAnalyzer analyzer)
        {
            function.ReturnType.BeginFulfilling();
            switch (function)
            {
                case INamedFunctionDeclarationSyntax namedFunction:
                    ResolveReturnType(function, namedFunction.ReturnTypeSyntax, analyzer);
                    return;
                default:
                    throw ExhaustiveMatch.Failed(function);
            }
        }

        private static void ResolveReturnType(
            IFunctionDeclarationSyntax function,
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
        private void ResolveSignatureTypesInClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            switch (classDeclaration.DeclaresType.State)
            {
                case PromiseState.InProgress:
                    diagnostics.Add(TypeError.CircularDefinition(classDeclaration.File, classDeclaration.NameSpan, classDeclaration.Name));
                    return;
                case PromiseState.Fulfilled:
                    return;   // We have already resolved it
                case PromiseState.Pending:
                    // we need to compute it
                    break;
                default:
                    throw ExhaustiveMatch.Failed(classDeclaration.DeclaresType.State);
            }

            classDeclaration.DeclaresType.BeginFulfilling();


            var classType = UserObjectType.Declaration(classDeclaration,
                classDeclaration.Modifiers.Any(m => m is IMutableKeywordToken));
            classDeclaration.DeclaresType.Fulfill(classType);
            classDeclaration.CreateDefaultConstructor();
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

        private void ResolveBodyTypesInDeclaration(IMemberDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case IFunctionDeclarationSyntax f:
                    ResolveBodyTypesInFunction(f);
                    break;
                case IFieldDeclarationSyntax f:
                    ResolveBodyTypesInField((FieldDeclarationSyntax)f);
                    break;
                case IConstructorDeclarationSyntax c:
                    ResolveBodyTypesInConstructor(c);
                    break;
                default:
                    throw ExhaustiveMatch.Failed(declaration);
            }
        }

        private void ResolveBodyTypesInConstructor(IConstructorDeclarationSyntax constructor)
        {
            if (constructor.Body == null)
                return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new BasicStatementAnalyzer(constructor.File, diagnostics,
                constructor.SelfParameterType, constructor.SelfParameterType);
            foreach (var statement in constructor.Body)
                resolver.ResolveTypesInStatement(statement);
            if (diagnosticCount != diagnostics.Count)
                constructor.MarkErrored();
        }

        private void ResolveBodyTypesInFunction(IFunctionDeclarationSyntax function)
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
