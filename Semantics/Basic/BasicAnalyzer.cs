using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;
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
        private readonly ITypeMetadata? stringSymbol;
        private readonly Diagnostics diagnostics;

        public BasicAnalyzer(ITypeMetadata? stringSymbol, Diagnostics diagnostics)
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
                    method.SelfParameterType = ResolveTypesInParameter(method.SelfParameter, method.DeclaringClass);
                    ResolveTypesInParameters(analyzer, method.Parameters);
                    ResolveReturnType(method.ReturnDataType, method.ReturnType, analyzer);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                {
                    var analyzer = new BasicTypeAnalyzer(constructor.File, diagnostics);
                    ResolveTypesInParameters(analyzer, constructor.Parameters);
                    break;
                }
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                {
                    var analyzer = new BasicTypeAnalyzer(associatedFunction.File, diagnostics);
                    ResolveTypesInParameters(analyzer, associatedFunction.Parameters);
                    ResolveReturnType(associatedFunction.ReturnDataType, associatedFunction.ReturnType, analyzer);
                    break;
                }
                case IFieldDeclarationSyntax field:
                    // Resolved by EntitySymbolResolver, just need to set metadata
                    var resolver = new BasicTypeAnalyzer(field.File, diagnostics);
                    resolver.Evaluate(field.TypeSyntax);
                    break;
                case IFunctionDeclarationSyntax function:
                {
                    var analyzer = new BasicTypeAnalyzer(function.File, diagnostics);
                    ResolveTypesInParameters(analyzer, function.Parameters);
                    ResolveReturnType(function.ReturnDataType, function.ReturnType, analyzer);
                    break;
                }
                case IClassDeclarationSyntax _:
                    // Resolved by EntitySymbolResolver
                    break;
            }
        }

        private static void ResolveTypesInParameters(
            BasicTypeAnalyzer analyzer,
            IEnumerable<IConstructorParameterSyntax> parameters)
        {
            foreach (var parameter in parameters)
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case INamedParameterSyntax namedParameter:
                        // It could already be fulfilled by the type resolver
                        if (parameter.DataType.TryBeginFulfilling())
                        {
                            var type = analyzer.Evaluate(namedParameter.TypeSyntax);
                            parameter.DataType.Fulfill(type);
                        }
                        else
                        {
                            // Still need to analyze to assign referenced metadata
                            analyzer.Evaluate(namedParameter.TypeSyntax);
                        }
                        break;
                    case IFieldParameterSyntax _:
                        // Resolved by EntitySymbolResolver
                        break;
                }
        }

        private static ObjectType ResolveTypesInParameter(
            ISelfParameterSyntax selfParameter,
            IClassDeclarationSyntax declaringClass)
        {
            var selfType = declaringClass.Symbol.Result.DeclaresDataType;
            if (selfParameter.DataType.TryBeginFulfilling())
            {
                if (selfParameter.MutableSelf) selfType = selfType.ForConstructorSelf();
                selfParameter.DataType.Fulfill(selfType);
            }
            return selfType;
        }

        private static void ResolveReturnType(
            Promise<DataType> returnTypePromise,
            ITypeSyntax? returnTypeSyntax,
            BasicTypeAnalyzer analyzer)
        {
            if (returnTypePromise.TryBeginFulfilling())
            {
                var returnType = returnTypeSyntax != null
                    ? analyzer.Evaluate(returnTypeSyntax) : DataType.Void;

                returnTypePromise.Fulfill(returnType);
            }
            else
                analyzer.Evaluate(returnTypeSyntax);
        }

        private void ResolveBodyTypes(IEntityDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IFunctionDeclarationSyntax function:
                {
                    var resolver = new BasicBodyAnalyzer(function.File, stringSymbol, diagnostics,
                        function.ReturnDataType.Result);
                    resolver.ResolveTypes(function.Body);
                    break;
                }
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                {
                    var resolver = new BasicBodyAnalyzer(associatedFunction.File, stringSymbol, diagnostics,
                        associatedFunction.ReturnDataType.Result);
                    resolver.ResolveTypes(associatedFunction.Body);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var resolver = new BasicBodyAnalyzer(method.File, stringSymbol, diagnostics, method.ReturnDataType.Result);
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
                        resolver.CheckType(ref field.Initializer, field.Symbol.Result.DataType);
                    }
                    break;
                case IConstructorDeclarationSyntax constructor:
                {
                    var resolver = new BasicBodyAnalyzer(constructor.File, stringSymbol, diagnostics, constructor.SelfParameterType);
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
