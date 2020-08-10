using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.EntitySymbols
{
    public class EntitySymbolResolver
    {
        private readonly Diagnostics diagnostics;
        private readonly SymbolTreeBuilder symbolTree;

        public EntitySymbolResolver(Diagnostics diagnostics, SymbolTreeBuilder symbolTree)
        {
            this.diagnostics = diagnostics;
            this.symbolTree = symbolTree;
        }

        public void Resolve(FixedList<IEntityDeclarationSyntax> entities)
        {
            // Process all classes first because they may be referenced by functions etc.
            foreach (var @class in entities.OfType<IClassDeclarationSyntax>())
                ResolveClass(@class);

            // Now resolve all other symbols (class declarations will already have symbols and won't be processed again)
            foreach (var entity in entities)
                ResolveEntity(entity);
        }

        /// <summary>
        /// If the type has not been resolved, this resolves it. This function
        /// also watches for type cycles and reports an error.
        /// </summary>
        private void ResolveEntity(IEntityDeclarationSyntax entity)
        {
            switch (entity)
            {
                default:
                    throw ExhaustiveMatch.Failed(entity);
                case IMethodDeclarationSyntax method:
                {
                    //var analyzer = new BasicTypeAnalyzer(method.File, diagnostics);
                    //method.SelfParameterType = ResolveTypesInParameter(method.SelfParameter, method.DeclaringClass);
                    //ResolveTypesInParameters(analyzer, method.Parameters, method.DeclaringClass);
                    //ResolveReturnType(method.ReturnDataType, method.ReturnType, analyzer);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                    ResolveConstructor(constructor);
                    break;
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                    ResolveAssociatedFunction(associatedFunction);
                    break;
                case IFieldDeclarationSyntax field:
                    ResolveField(field);
                    break;
                case IFunctionDeclarationSyntax syn:
                    ResolveFunction(syn);
                    break;
                case IClassDeclarationSyntax syn:
                    ResolveClass(syn);
                    break;
            }
        }

        private void ResolveConstructor(IConstructorDeclarationSyntax constructor)
        {
            var selfType = constructor.DeclaringClass.Symbol.Result.DeclaresDataType;
            constructor.SelfParameterType = ResolveTypesInParameter(constructor.ImplicitSelfParameter, constructor.DeclaringClass);
            var resolver = new TypeResolver(constructor.File, diagnostics);
            ResolveTypesInParameters(resolver, constructor.Parameters, constructor.DeclaringClass);
            // TODO deal with return type here
        }

        private void ResolveAssociatedFunction(IAssociatedFunctionDeclarationSyntax associatedFunction)
        {
            var resolver = new TypeResolver(associatedFunction.File, diagnostics);
            ResolveTypesInParameters(resolver, associatedFunction.Parameters, null);
            ResolveReturnType(associatedFunction.ReturnDataType, associatedFunction.ReturnType, resolver);
        }

        private void ResolveField(IFieldDeclarationSyntax field)
        {
            if (!field.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

            var resolver = new TypeResolver(field.File, diagnostics);
            var type = resolver.Evaluate(field.TypeSyntax);
            var symbol = new FieldSymbol(field.DeclaringClass.Symbol.Result, field.Name, field.IsMutableBinding, type);
            field.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);

            void AddCircularDefinitionError()
            {
                diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan, field.Name.ToSimpleName()));
            }
        }

        private void ResolveFunction(IFunctionDeclarationSyntax function)
        {
            var resolver = new TypeResolver(function.File, diagnostics);
            ResolveTypesInParameters(resolver, function.Parameters, null);
            ResolveReturnType(function.ReturnDataType, function.ReturnType, resolver);
        }

        private void ResolveTypesInParameters(
            TypeResolver resolver,
            IEnumerable<IConstructorParameterSyntax> parameters,
            IClassDeclarationSyntax? declaringClass)
        {
            foreach (var parameter in parameters)
                switch (parameter)
                {
                    default:
                        throw ExhaustiveMatch.Failed(parameter);
                    case INamedParameterSyntax namedParameter:
                    {
                        parameter.DataType.BeginFulfilling();
                        var type = resolver.Evaluate(namedParameter.TypeSyntax);
                        parameter.DataType.Fulfill(type);
                    }
                    break;
                    case IFieldParameterSyntax fieldParameter:
                    {
                        parameter.DataType.BeginFulfilling();
                        var field = (declaringClass ?? throw new InvalidOperationException("Field parameter outside of class declaration"))
                                    .Members.OfType<IFieldDeclarationSyntax>()
                                    .SingleOrDefault(f => f.Name == fieldParameter.Name);
                        if (field is null)
                        {
                            fieldParameter.SetIsMutableBinding(false);
                            fieldParameter.DataType.Fulfill(DataType.Unknown);
                            // TODO report an error
                            throw new NotImplementedException();
                        }
                        else
                        {
                            fieldParameter.SetIsMutableBinding(field.IsMutableBinding);
                            ResolveField(field);
                            parameter.DataType.Fulfill(field.Symbol.Result.DataType);
                        }
                    }
                    break;
                }
        }

        private static ObjectType ResolveTypesInParameter(
            ISelfParameterSyntax selfParameter,
            IClassDeclarationSyntax declaringClass)
        {
            var selfType = declaringClass.Symbol.Result.DeclaresDataType;
            selfParameter.DataType.BeginFulfilling();
            if (selfParameter.MutableSelf) selfType = selfType.ForConstructorSelf();
            selfParameter.DataType.Fulfill(selfType);
            return selfType;
        }

        private static void ResolveReturnType(
            Promise<DataType> returnTypePromise,
            ITypeSyntax? returnTypeSyntax,
            TypeResolver resolver)
        {
            returnTypePromise.BeginFulfilling();
            var returnType = returnTypeSyntax != null
                ? resolver.Evaluate(returnTypeSyntax) : DataType.Void;
            returnTypePromise.Fulfill(returnType);
        }

        private void ResolveClass(IClassDeclarationSyntax @class)
        {
            if (!@class.Symbol.TryBeginFulfilling(AddCircularDefinitionError))
                return;

            bool mutable = !(@class.MutableModifier is null);
            var classType = new ObjectType(@class.ContainingNamespaceName, @class.Name, mutable, ReferenceCapability.Shared);

            var symbol = new ObjectTypeSymbol(@class.ContainingNamespaceSymbol!, classType);
            @class.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
            var defaultConstructorSymbol = @class.CreateDefaultConstructor();
            if (!(defaultConstructorSymbol is null))
                symbolTree.Add(defaultConstructorSymbol);

            void AddCircularDefinitionError()
            {
                // TODO use something better than Name here which is an old name
                diagnostics.Add(TypeError.CircularDefinition(@class.File, @class.NameSpan, @class.Name.ToSimpleName()));
            }
        }
    }
}
