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

        public static void Check(FixedList<IEntityDeclarationSyntax> memberDeclarations, Diagnostics diagnostics)
        {
            var analyzer = new BasicAnalyzer(diagnostics);
            analyzer.ResolveTypesInDeclarations(memberDeclarations);
        }

        private void ResolveTypesInDeclarations(FixedList<IEntityDeclarationSyntax> declarations)
        {
            // Process all types first because they may be referenced by functions etc.
            ResolveSignatureTypesInClassDeclarations(declarations.OfType<IClassDeclarationSyntax>());
            // Now resolve all other types (class declarations will already have types and won't be processed again)
            ResolveSignatureTypesInDeclarations(declarations);
            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            ResolveBodyTypesInDeclarations(declarations);
        }

        private void ResolveSignatureTypesInClassDeclarations(IEnumerable<IClassDeclarationSyntax> classDeclarations)
        {
            foreach (var classDeclaration in classDeclarations)
                ResolveSignatureTypesInClassDeclaration(classDeclaration);
        }

        private void ResolveSignatureTypesInDeclarations(IEnumerable<IEntityDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveSignatureTypesInDeclaration(declaration);
        }

        private void ResolveSignatureTypesInDeclaration(IEntityDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IMethodDeclarationSyntax f:
                    ResolveSignatureTypesInFunction(f);
                    break;
                case IConstructorDeclarationSyntax c:
                    ResolveSignatureTypesInConstructor(c);
                    break;
                case IFieldDeclarationSyntax f:
                    ResolveSignatureTypesInField(f);
                    break;
                case IClassDeclarationSyntax _:
                    // Already processed
                    break;
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


        private void ResolveSignatureTypesInFunction(IMethodDeclarationSyntax method)
        {
            var diagnosticCount = diagnostics.Count;

            // Resolve the declaring type because we need its type for things like `self`
            if (method.DeclaringType != null)
                ResolveSignatureTypesInClassDeclaration(method.DeclaringType);

            var selfType = ResolveSelfType(method);
            var analyzer = new BasicStatementAnalyzer(method.File, diagnostics, selfType);

            ResolveTypesInParameters(analyzer, method.Parameters, method.DeclaringType);

            ResolveReturnType(method, analyzer);

            if (diagnosticCount != diagnostics.Count)
                method.MarkErrored();
        }

        private static DataType? ResolveSelfType(IMethodDeclarationSyntax method)
        {
            var declaringType = method.DeclaringType?.DeclaresType.Fulfilled();
            if (declaringType == null)
                return null;

            switch (method)
            {
                default:
                    throw ExhaustiveMatch.Failed(method);
                case IMethodDeclarationSyntax namedFunction:
                    var selfParameter = namedFunction.Parameters.OfType<ISelfParameterSyntax>().SingleOrDefault();
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
            FixedList<IParameterSyntax> parameters,
            IClassDeclarationSyntax declaringType)
        {
            var types = new List<DataType>();
            foreach (var parameter in parameters)
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case INamedParameterSyntax namedParameter:
                    {
                        parameter.Type.BeginFulfilling();
                        var type = analyzer
                            .EvaluateType(namedParameter.TypeSyntax);
                        types.Add(parameter.Type.Fulfill(type));
                    }
                    break;
                    case ISelfParameterSyntax _:
                        // Skip, we have already handled the self parameter
                        break;
                    case IFieldParameterSyntax fieldParameter:
                        parameter.Type.BeginFulfilling();
                        var field = declaringType.Members
                            .OfType<IFieldDeclarationSyntax>()
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
                }

            types.ToFixedList();
        }

        private static void ResolveReturnType(
            IMethodDeclarationSyntax method,
            BasicStatementAnalyzer analyzer)
        {
            method.ReturnType.BeginFulfilling();
            ResolveReturnType(method, method.ReturnTypeSyntax, analyzer);
        }

        private static void ResolveReturnType(
            IMethodDeclarationSyntax method,
            TypeSyntax returnTypeSyntax,
            BasicStatementAnalyzer analyzer)
        {
            var returnType = returnTypeSyntax != null
                ? analyzer.EvaluateType(returnTypeSyntax)
                : DataType.Void;

            // If we are returning ownership, then they can make it mutable
            if (returnType is UserObjectType objectType && objectType.IsOwned)
                returnType = objectType.AsImplicitlyUpgradable();
            method.ReturnType.Fulfill(returnType);
        }

        /// <summary>
        /// If the type has not been resolved, this resolves it. This function
        /// also watches for type cycles and reports an error.
        /// </summary>
        private void ResolveSignatureTypesInClassDeclaration(IClassDeclarationSyntax classDeclaration)
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

        private void ResolveSignatureTypesInField(IFieldDeclarationSyntax field)
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
             IEnumerable<IEntityDeclarationSyntax> declarations)
        {
            foreach (var declaration in declarations)
                ResolveBodyTypesInDeclaration(declaration);
        }

        private void ResolveBodyTypesInDeclaration(IEntityDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IMethodDeclarationSyntax f:
                    ResolveBodyTypesInFunction(f);
                    break;
                case IFieldDeclarationSyntax f:
                    ResolveBodyTypesInField(f);
                    break;
                case IConstructorDeclarationSyntax c:
                    ResolveBodyTypesInConstructor(c);
                    break;
                case IClassDeclarationSyntax _:
                    // body of class is processed as separate items
                    break;
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

        private void ResolveBodyTypesInFunction(IMethodDeclarationSyntax method)
        {
            if (method.Body == null)
                return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new BasicStatementAnalyzer(method.File, diagnostics, method.SelfParameterType, method.ReturnType.Fulfilled());
            foreach (var statement in method.Body)
                resolver.ResolveTypesInStatement(statement);
            if (diagnosticCount != diagnostics.Count)
                method.MarkErrored();
        }

        private void ResolveBodyTypesInField(IFieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Initializer == null)
                return;

            var diagnosticCount = diagnostics.Count;
            var resolver = new BasicStatementAnalyzer(fieldDeclaration.File, diagnostics);
            // Work around not being able to pass a ref to a property
            resolver.CheckExpressionType(ref fieldDeclaration.InitializerRef, fieldDeclaration.Type.Fulfilled());
            if (diagnosticCount != diagnostics.Count)
                fieldDeclaration.MarkErrored();
        }
    }
}
