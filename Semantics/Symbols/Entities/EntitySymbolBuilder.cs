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

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Entities
{
    public class EntitySymbolBuilder
    {
        private readonly Diagnostics diagnostics;
        private readonly SymbolTreeBuilder symbolTree;

        public EntitySymbolBuilder(Diagnostics diagnostics, SymbolTreeBuilder symbolTree)
        {
            this.diagnostics = diagnostics;
            this.symbolTree = symbolTree;
        }

        public void Build(FixedList<IEntityDeclarationSyntax> entities)
        {
            // Process all classes first because they may be referenced by functions etc.
            foreach (var @class in entities.OfType<IClassDeclarationSyntax>())
                BuildClassSymbol(@class);

            // Now resolve all other symbols (class declarations will already have symbols and won't be processed again)
            foreach (var entity in entities)
                BuildEntitySymbol(entity);
        }

        /// <summary>
        /// If the type has not been resolved, this resolves it. This function
        /// also watches for type cycles and reports an error.
        /// </summary>
        private void BuildEntitySymbol(IEntityDeclarationSyntax entity)
        {
            switch (entity)
            {
                default:
                    throw ExhaustiveMatch.Failed(entity);
                case IMethodDeclarationSyntax method:
                    BuildMethodSymbol(method);
                    break;
                case IConstructorDeclarationSyntax constructor:
                    BuildConstructorSymbol(constructor);
                    break;
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                    BuildAssociatedFunctionSymbol(associatedFunction);
                    break;
                case IFieldDeclarationSyntax field:
                    BuildFieldSymbol(field);
                    break;
                case IFunctionDeclarationSyntax syn:
                    BuildFunctionSymbol(syn);
                    break;
                case IClassDeclarationSyntax syn:
                    BuildClassSymbol(syn);
                    break;
            }
        }

        private void BuildMethodSymbol(IMethodDeclarationSyntax method)
        {
            method.Symbol.BeginFulfilling();
            var declaringClassSymbol = method.DeclaringClass.Symbol.Result;
            var resolver = new TypeResolver(method.File, diagnostics);
            var selfParameterType = ResolveSelfParameterType(method.SelfParameter, method.DeclaringClass);
            var parameterTypes = ResolveParameterTypes(resolver, method.Parameters, method.DeclaringClass);
            var returnType = ResolveReturnType(method.ReturnDataType, method.ReturnType, resolver);
            var symbol = new MethodSymbol(declaringClassSymbol, method.Name, selfParameterType, parameterTypes, returnType);
            method.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
            BuildSelParameterSymbol(symbol, method.SelfParameter, selfParameterType);
            BuildParameterSymbols(symbol, method.Parameters, parameterTypes);
        }

        private void BuildConstructorSymbol(IConstructorDeclarationSyntax constructor)
        {
            constructor.Symbol.BeginFulfilling();
            var selfParameterType = ResolveSelfParameterType(constructor.ImplicitSelfParameter, constructor.DeclaringClass);
            var resolver = new TypeResolver(constructor.File, diagnostics);
            var parameterTypes = ResolveParameterTypes(resolver, constructor.Parameters, constructor.DeclaringClass);
            var declaringClassSymbol = constructor.DeclaringClass.Symbol.Result;
            var symbol = new ConstructorSymbol(declaringClassSymbol, constructor.Name, parameterTypes);
            constructor.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
            BuildSelParameterSymbol(symbol, constructor.ImplicitSelfParameter, selfParameterType);
            BuildParameterSymbols(symbol, constructor.Parameters, parameterTypes);
        }

        private void BuildAssociatedFunctionSymbol(IAssociatedFunctionDeclarationSyntax associatedFunction)
        {
            associatedFunction.Symbol.BeginFulfilling();
            var resolver = new TypeResolver(associatedFunction.File, diagnostics);
            var parameterTypes = ResolveParameterTypes(resolver, associatedFunction.Parameters, null);
            var returnType = ResolveReturnType(associatedFunction.ReturnDataType, associatedFunction.ReturnType, resolver);
            var declaringClassSymbol = associatedFunction.DeclaringClass.Symbol.Result;
            var symbol = new FunctionSymbol(declaringClassSymbol, associatedFunction.Name, parameterTypes, returnType);
            associatedFunction.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
            BuildParameterSymbols(symbol, associatedFunction.Parameters, parameterTypes);
        }

        private void BuildFieldSymbol(IFieldDeclarationSyntax field)
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

        private void BuildFunctionSymbol(IFunctionDeclarationSyntax function)
        {
            function.Symbol.BeginFulfilling();
            var resolver = new TypeResolver(function.File, diagnostics);
            var parameterTypes = ResolveParameterTypes(resolver, function.Parameters, null);
            var returnType = ResolveReturnType(function.ReturnDataType, function.ReturnType, resolver);
            var symbol = new FunctionSymbol(function.ContainingNamespaceSymbol, function.Name, parameterTypes, returnType);
            function.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
            BuildParameterSymbols(symbol, function.Parameters, parameterTypes);
        }

        private void BuildClassSymbol(IClassDeclarationSyntax @class)
        {
            if (!@class.Symbol.TryBeginFulfilling(AddCircularDefinitionError)) return;

            bool mutable = !(@class.MutableModifier is null);
            var classType = new ObjectType(@class.ContainingNamespaceName, @class.Name, mutable,
                ReferenceCapability.Shared);

            var symbol = new ObjectTypeSymbol(@class.ContainingNamespaceSymbol!, classType);
            @class.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
            var defaultConstructorSymbol = @class.CreateDefaultConstructor();
            if (!(defaultConstructorSymbol is null)) symbolTree.Add(defaultConstructorSymbol);

            void AddCircularDefinitionError()
            {
                // TODO use something better than Name here which is an old name
                diagnostics.Add(TypeError.CircularDefinition(@class.File, @class.NameSpan, @class));
            }
        }

        private FixedList<DataType> ResolveParameterTypes(
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
                        var type = resolver.Evaluate(namedParameter.TypeSyntax);
                        types.Add(type);
                    }
                    break;
                    case IFieldParameterSyntax fieldParameter:
                    {
                        var field = (declaringClass ?? throw new InvalidOperationException("Field parameter outside of class declaration"))
                                    .Members.OfType<IFieldDeclarationSyntax>()
                                    .SingleOrDefault(f => f.Name == fieldParameter.Name);
                        if (field is null)
                        {
                            fieldParameter.SetIsMutableBinding(false);
                            types.Add(DataType.Unknown);
                            // TODO report an error
                            throw new NotImplementedException();
                        }
                        else
                        {
                            fieldParameter.SetIsMutableBinding(field.IsMutableBinding);
                            BuildFieldSymbol(field);
                            types.Add(field.Symbol.Result.DataType);
                        }
                    }
                    break;
                }

            return types.ToFixedList();
        }

        private void BuildParameterSymbols(
            InvokableSymbol containingSymbol,
            IEnumerable<IConstructorParameterSyntax> parameters,
            IEnumerable<DataType> types)
        {
            foreach (var (param, type) in parameters.Zip(types))
            {
                param.Symbol.BeginFulfilling();
                var symbol = new VariableSymbol(containingSymbol, param.Name, 0, param.IsMutableBinding, type);
                param.Symbol.Fulfill(symbol);
                symbolTree.Add(symbol);
            }
        }

        private static ObjectType ResolveSelfParameterType(
            ISelfParameterSyntax selfParameter,
            IClassDeclarationSyntax declaringClass)
        {
            var selfType = declaringClass.Symbol.Result.DeclaresDataType;
            if (selfParameter.MutableSelf)
                selfType = selfType.ForConstructorSelf();
            return selfType;
        }

        private void BuildSelParameterSymbol(
            InvokableSymbol containingSymbol,
            ISelfParameterSyntax param,
            DataType type)
        {
            param.Symbol.BeginFulfilling();
            var symbol = new SelfParameterSymbol(containingSymbol, param.IsMutableBinding, type);
            param.Symbol.Fulfill(symbol);
            symbolTree.Add(symbol);
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
    }
}
