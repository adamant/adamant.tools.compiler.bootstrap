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

        public BasicAnalyzer(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public void Analyze(FixedList<IEntityDeclarationSyntax> entities)
        {
            // Process all types first because they may be referenced by functions etc.
            foreach (var @class in entities.OfType<IClassDeclarationSyntax>())
                ResolveSignatureTypes(@class);

            // Now resolve all other types (class declarations will already have types and won't be processed again)
            foreach (var entity in entities)
                ResolveSignatureTypes(entity);

            // Function bodies are checked after signatures to ensure that all function invocation
            // expressions can get a type for the invoked function.
            foreach (var entity in entities)
                ResolveBodyTypes(entity);
        }

        /// <summary>
        /// If the type has not been resolved, this resolves it. This function
        /// also watches for type cycles and reports an error.
        /// </summary>
        private void ResolveSignatureTypes(IEntityDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IMethodDeclarationSyntax method:
                {
                    var selfType = ResolveSelfType(method);
                    var analyzer = new BasicStatementAnalyzer(method.File, diagnostics, selfType);
                    ResolveTypesInParameters(analyzer, method.Parameters, method.DeclaringClass);
                    ResolveReturnType(method.ReturnType, method.ReturnTypeSyntax, analyzer);
                    break;
                }
                case IConstructorDeclarationSyntax @class:
                {
                    var selfType = @class.DeclaringClass?.DeclaresType.Fulfilled();
                    @class.SelfParameterType = ((UserObjectType)selfType).ForConstructorSelf();
                    var analyzer = new BasicStatementAnalyzer(@class.File, diagnostics, selfType);
                    ResolveTypesInParameters(analyzer, @class.Parameters, @class.DeclaringClass);
                    break;
                }
                case IFieldDeclarationSyntax field:
                    if (field.Type.TryBeginFulfilling(() =>
                        diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan, field.Name))))
                    {
                        var resolver = new BasicStatementAnalyzer(field.File, diagnostics);
                        field.Type.BeginFulfilling();
                        var type = resolver.EvaluateType(field.TypeSyntax);
                        field.Type.Fulfill(type);
                    }
                    break;
                case IFunctionDeclarationSyntax function:
                {
                    var analyzer = new BasicStatementAnalyzer(function.File, diagnostics);
                    ResolveTypesInParameters(analyzer, function.Parameters, null);
                    ResolveReturnType(function.ReturnType, function.ReturnTypeSyntax, analyzer);
                    break;
                }
                case IClassDeclarationSyntax c:
                    if (c.DeclaresType.TryBeginFulfilling(() => diagnostics.Add(
                        TypeError.CircularDefinition(c.File, c.NameSpan, c.Name))))
                    {
                        var classType = UserObjectType.Declaration(c,
                            c.Modifiers.Any(m => m is IMutableKeywordToken));
                        c.DeclaresType.Fulfill(classType);
                        c.CreateDefaultConstructor();
                    }
                    break;
            }
        }

        private static DataType? ResolveSelfType(IMethodDeclarationSyntax method)
        {
            var declaringType = method.DeclaringClass?.DeclaresType.Fulfilled();
            if (declaringType == null)
                return null;

            var selfParameter = method.Parameters.OfType<ISelfParameterSyntax>().SingleOrDefault();
            if (selfParameter == null)
                return null; // Static function
            selfParameter.Type.BeginFulfilling();
            var selfType = (UserObjectType)declaringType;
            if (selfParameter.MutableSelf)
                selfType = selfType.AsMutable();
            return method.SelfParameterType = selfParameter.Type.Fulfill(selfType);
        }

        private void ResolveTypesInParameters(
            BasicStatementAnalyzer analyzer,
            FixedList<IParameterSyntax> parameters,
            IClassDeclarationSyntax? declaringType)
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
                            if (field.Type.TryBeginFulfilling(() =>
                                diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan, field.Name))))
                            {
                                var resolver = new BasicStatementAnalyzer(field.File, diagnostics);
                                field.Type.BeginFulfilling();
                                var type = resolver.EvaluateType(field.TypeSyntax);
                                field.Type.Fulfill(type);
                            }

                            parameter.Type.Fulfill(field.Type.Fulfilled());
                        }
                        break;
                }

            types.ToFixedList();
        }

        private static void ResolveReturnType(
            TypePromise returnTypePromise,
            ITypeSyntax? returnTypeSyntax,
            BasicStatementAnalyzer analyzer)
        {
            returnTypePromise.BeginFulfilling();
            var returnType = returnTypeSyntax != null
                ? analyzer.EvaluateType(returnTypeSyntax)
                : DataType.Void;

            // If we are returning ownership, then they can make it mutable
            if (returnType is UserObjectType objectType && objectType.IsOwned)
                returnType = objectType.AsImplicitlyUpgradable();
            returnTypePromise.Fulfill(returnType);
        }

        private void ResolveBodyTypes(IEntityDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IFunctionDeclarationSyntax function:
                {
                    var resolver = new BasicStatementAnalyzer(function.File, diagnostics, null, function.ReturnType.Fulfilled());
                    foreach (var statement in function.Body.Statements)
                        resolver.ResolveTypesInStatement(statement);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var resolver = new BasicStatementAnalyzer(method.File, diagnostics, method.SelfParameterType, method.ReturnType.Fulfilled());
                    foreach (var statement in method.Body.Statements)
                        resolver.ResolveTypesInStatement(statement);
                    break;
                }
                case IAbstractMethodDeclarationSyntax _:
                    // has no body, so nothing to resolve
                    break;
                case IFieldDeclarationSyntax field:
                    if (field.Initializer != null)
                    {
                        var resolver = new BasicStatementAnalyzer(field.File, diagnostics);
                        // Work around not being able to pass a ref to a property
                        resolver.CheckExpressionType(ref field.Initializer, field.Type.Fulfilled());
                    }
                    break;
                case IConstructorDeclarationSyntax constructor:
                {
                    var resolver = new BasicStatementAnalyzer(constructor.File, diagnostics, constructor.SelfParameterType, constructor.SelfParameterType);
                    foreach (var statement in constructor.Body.Statements)
                        resolver.ResolveTypesInStatement(statement);
                    break;
                }
                case IClassDeclarationSyntax _:
                    // body of class is processed as separate items
                    break;
            }
        }
    }
}
