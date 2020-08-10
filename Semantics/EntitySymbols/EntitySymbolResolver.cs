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
                    ResolveMethod(method);
                    break;
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

        private void ResolveMethod(IMethodDeclarationSyntax method)
        {
            method.Symbol.BeginFulfilling();
            var resolver = new TypeResolver(method.File, diagnostics);
            var selfParameterType = ResolveTypesInParameter(method.SelfParameter, method.DeclaringClass);
            method.SelfParameterType = selfParameterType;
            var parameterTypes = ResolveTypesInParameters(resolver, method.Parameters, method.DeclaringClass);
            var returnType = ResolveReturnType(method.ReturnDataType, method.ReturnType, resolver);
            var declaringClassSymbol = method.DeclaringClass.Symbol.Result;
            var symbol = new MethodSymbol(declaringClassSymbol, method.Name, selfParameterType, parameterTypes, returnType);
            method.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
        }

        private void ResolveConstructor(IConstructorDeclarationSyntax constructor)
        {
            constructor.Symbol.BeginFulfilling();
            constructor.SelfParameterType = ResolveTypesInParameter(constructor.ImplicitSelfParameter, constructor.DeclaringClass);
            var resolver = new TypeResolver(constructor.File, diagnostics);
            var parameterTypes = ResolveTypesInParameters(resolver, constructor.Parameters, constructor.DeclaringClass);
            var declaringClassSymbol = constructor.DeclaringClass.Symbol.Result;
            var symbol = new ConstructorSymbol(declaringClassSymbol, constructor.Name, parameterTypes);
            constructor.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
        }

        private void ResolveAssociatedFunction(IAssociatedFunctionDeclarationSyntax associatedFunction)
        {
            associatedFunction.Symbol.BeginFulfilling();
            var resolver = new TypeResolver(associatedFunction.File, diagnostics);
            var parameterTypes = ResolveTypesInParameters(resolver, associatedFunction.Parameters, null);
            var returnType = ResolveReturnType(associatedFunction.ReturnDataType, associatedFunction.ReturnType, resolver);
            var declaringClassSymbol = associatedFunction.DeclaringClass.Symbol.Result;
            var symbol = new FunctionSymbol(declaringClassSymbol, associatedFunction.Name, parameterTypes, returnType);
            associatedFunction.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
        }

        private void ResolveField(IFieldDeclarationSyntax field)
        {
            if (field.Symbol.State == PromiseState.Fulfilled)
                return;

            field.Symbol.BeginFulfilling();
            var resolver = new TypeResolver(field.File, diagnostics);
            var type = resolver.Evaluate(field.TypeSyntax);
            var symbol = new FieldSymbol(field.DeclaringClass.Symbol.Result, field.Name, field.IsMutableBinding, type);
            field.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
        }

        private void ResolveFunction(IFunctionDeclarationSyntax function)
        {
            function.Symbol.BeginFulfilling();
            var resolver = new TypeResolver(function.File, diagnostics);
            var parameterTypes = ResolveTypesInParameters(resolver, function.Parameters, null);
            var returnType = ResolveReturnType(function.ReturnDataType, function.ReturnType, resolver);
            var symbol = new FunctionSymbol(function.ContainingNamespaceSymbol, function.Name, parameterTypes, returnType);
            function.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
        }

        private FixedList<DataType> ResolveTypesInParameters(
            TypeResolver resolver,
            IEnumerable<IConstructorParameterSyntax> parameters,
            IClassDeclarationSyntax? declaringClass)
        {
            var types = new List<DataType>();
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
                        types.Add(type);
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
                        types.Add(fieldParameter.DataType.Result);
                    }
                    break;
                }

            return types.ToFixedList();
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

        private static DataType ResolveReturnType(
            Promise<DataType> returnTypePromise,
            ITypeSyntax? returnTypeSyntax,
            TypeResolver resolver)
        {
            returnTypePromise.BeginFulfilling();
            var returnType = returnTypeSyntax != null
                ? resolver.Evaluate(returnTypeSyntax) : DataType.Void;
            returnTypePromise.Fulfill(returnType);
            return returnType;
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
                diagnostics.Add(TypeError.CircularDefinition(@class.File, @class.NameSpan, @class));
            }
        }
    }
}
