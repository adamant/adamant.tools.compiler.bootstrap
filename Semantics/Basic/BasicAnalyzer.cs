using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
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
    /// * Evaluate - determine the type for some type syntax
    /// </summary>
    public class BasicAnalyzer
    {
        private readonly ITypeSymbol? stringSymbol;
        private readonly Diagnostics diagnostics;

        public BasicAnalyzer(ITypeSymbol? stringSymbol, Diagnostics diagnostics)
        {
            this.stringSymbol = stringSymbol;
            this.diagnostics = diagnostics;
        }

        public void Check(FixedList<IEntityDeclarationSyntax> entities)
        {
            // Process all classes first because they may be referenced by functions etc.
            foreach (var @class in entities.OfType<IClassDeclarationSyntax>())
                ResolveSignatureTypes(@class);

            // Now resolve all other signature types (class declarations will already have types and won't be processed again)
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
                    var analyzer = new BasicTypeAnalyzer(method.File, diagnostics);
                    method.SelfParameterType = ResolveTypesInParameters(analyzer, method.Parameters, method.DeclaringClass)
                        ?? throw new InvalidOperationException("Method doesn't have self parameter");
                    ResolveReturnType(method.ReturnType, method.ReturnTypeSyntax, analyzer);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                {
                    var selfType = constructor.DeclaringClass.DeclaresType.Fulfilled();
                    constructor.SelfParameterType = ((UserObjectType)selfType).ForConstructorSelf();
                    var analyzer = new BasicTypeAnalyzer(constructor.File, diagnostics);
                    ResolveTypesInParameters(analyzer, constructor.Parameters, constructor.DeclaringClass);
                    // TODO deal with return type here
                    break;
                }
                case IAssociatedFunctionDeclaration associatedFunction:
                {
                    var analyzer = new BasicTypeAnalyzer(associatedFunction.File, diagnostics);
                    ResolveTypesInParameters(analyzer, associatedFunction.Parameters, null);
                    ResolveReturnType(associatedFunction.ReturnType, associatedFunction.ReturnTypeSyntax, analyzer);
                    break;
                }
                case IFieldDeclarationSyntax field:
                    if (field.Type.TryBeginFulfilling(() =>
                        diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan, field.Name))))
                    {
                        var resolver = new BasicTypeAnalyzer(field.File, diagnostics);
                        var type = resolver.Evaluate(field.TypeSyntax);
                        field.Type.Fulfill(type);
                    }
                    break;
                case IFunctionDeclarationSyntax function:
                {
                    var analyzer = new BasicTypeAnalyzer(function.File, diagnostics);
                    ResolveTypesInParameters(analyzer, function.Parameters, null);
                    ResolveReturnType(function.ReturnType, function.ReturnTypeSyntax, analyzer);
                    break;
                }
                case IClassDeclarationSyntax @class:
                    if (@class.DeclaresType.TryBeginFulfilling(() => diagnostics.Add(
                        TypeError.CircularDefinition(@class.File, @class.NameSpan, @class.Name))))
                    {
                        var classType = UserObjectType.Declaration(@class,
                            @class.Modifiers.Any(m => m is IMutableKeywordToken));
                        @class.DeclaresType.Fulfill(classType);
                        @class.CreateDefaultConstructor();
                    }
                    break;
            }
        }

        private UserObjectType? ResolveTypesInParameters(
            BasicTypeAnalyzer analyzer,
            IEnumerable<IParameterSyntax> parameters,
            IClassDeclarationSyntax? declaringClass)
        {
            UserObjectType? selfType = null;
            foreach (var parameter in parameters)
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case INamedParameterSyntax namedParameter:
                    {
                        parameter.Type.BeginFulfilling();
                        var type = analyzer.Evaluate(namedParameter.TypeSyntax);
                        parameter.Type.Fulfill(type);
                    }
                    break;
                    case ISelfParameterSyntax selfParameter:
                    {
                        var declaringType = declaringClass?.DeclaresType.Fulfilled()
                                            ?? throw new InvalidOperationException("Self parameter outside of class declaration");
                        selfParameter.Type.BeginFulfilling();
                        selfType = (UserObjectType)declaringType;
                        if (selfParameter.MutableSelf) selfType = selfType.ToMutable();
                        selfParameter.Type.Fulfill(selfType);
                    }
                    break;
                    case IFieldParameterSyntax fieldParameter:
                    {
                        parameter.Type.BeginFulfilling();
                        var field = (declaringClass?? throw new InvalidOperationException("Field parameter outside of class declaration"))
                                    .Members.OfType<IFieldDeclarationSyntax>()
                                    .SingleOrDefault(f => f.Name == fieldParameter.FieldName);
                        if (field is null)
                        {
                            parameter.Type.Fulfill(DataType.Unknown);
                            // TODO report an error
                            throw new NotImplementedException();
                        }
                        else
                        {
                            if (field.Type.TryBeginFulfilling(() =>
                                diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan,
                                    field.Name))))
                            {
                                var resolver = new BasicBodyAnalyzer(field.File, stringSymbol, diagnostics);
                                field.Type.BeginFulfilling();
                                var type = resolver.EvaluateType(field.TypeSyntax);
                                field.Type.Fulfill(type);
                            }

                            parameter.Type.Fulfill(field.Type.Fulfilled());
                        }
                    }
                    break;
                }

            return selfType;
        }

        private static void ResolveReturnType(
            TypePromise returnTypePromise,
            ITypeSyntax? returnTypeSyntax,
            BasicTypeAnalyzer analyzer)
        {
            returnTypePromise.BeginFulfilling();
            var returnType = returnTypeSyntax != null
                ? analyzer.Evaluate(returnTypeSyntax)
                : DataType.Void;

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
                    var resolver = new BasicBodyAnalyzer(function.File, stringSymbol, diagnostics, null,
                        function.ReturnType.Fulfilled());
                    resolver.ResolveTypes(function.Body);
                    break;
                }
                case IAssociatedFunctionDeclaration associatedFunction:
                {
                    var resolver = new BasicBodyAnalyzer(associatedFunction.File, stringSymbol, diagnostics, null,
                        associatedFunction.ReturnType.Fulfilled());
                    resolver.ResolveTypes(associatedFunction.Body);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var resolver = new BasicBodyAnalyzer(method.File, stringSymbol, diagnostics,
                        method.SelfParameterType, method.ReturnType.Fulfilled());
                    resolver.ResolveTypes(method.Body);
                    break;
                }
                case IAbstractMethodDeclarationSyntax _:
                    // has no body, so nothing to resolve
                    break;
                case IFieldDeclarationSyntax field:
                    if (field.Initializer != null)
                    {
                        var resolver = new BasicBodyAnalyzer(field.File, stringSymbol, diagnostics);
                        resolver.CheckType(ref field.Initializer, field.Type.Fulfilled());
                    }
                    break;
                case IConstructorDeclarationSyntax constructor:
                {
                    var resolver = new BasicBodyAnalyzer(constructor.File, stringSymbol, diagnostics,
                        constructor.SelfParameterType, constructor.SelfParameterType);
                    resolver.ResolveTypes(constructor.Body);
                    break;
                }
                case IClassDeclarationSyntax _:
                    // body of class is processed as separate items
                    break;
            }
        }
    }
}
