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
        private readonly SymbolTree symbolTree;

        public EntitySymbolResolver(Diagnostics diagnostics, SymbolTree symbolTree)
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
                {
                    //var selfType = constructor.DeclaringClass.DeclaresDataType.Fulfilled();
                    //constructor.SelfParameterType = ResolveTypesInParameter(constructor.ImplicitSelfParameter, constructor.DeclaringClass);
                    //var analyzer = new BasicTypeAnalyzer(constructor.File, diagnostics);
                    //ResolveTypesInParameters(analyzer, constructor.Parameters, constructor.DeclaringClass);
                    //// TODO deal with return type here
                    break;
                }
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                {
                    //var analyzer = new BasicTypeAnalyzer(associatedFunction.File, diagnostics);
                    //ResolveTypesInParameters(analyzer, associatedFunction.Parameters, null);
                    //ResolveReturnType(associatedFunction.ReturnDataType, associatedFunction.ReturnType, analyzer);
                    break;
                }
                case IFieldDeclarationSyntax field:
                    //if (field.DataType.TryBeginFulfilling(() =>
                    //    diagnostics.Add(TypeError.CircularDefinition(field.File, field.NameSpan, field.Name.ToSimpleName()))))
                    //{
                    //    var resolver = new TypeResolver(field.File, diagnostics);
                    //    var type = resolver.Evaluate(field.TypeSyntax);
                    //    field.DataType.Fulfill(type);
                    //}
                    break;
                case IFunctionDeclarationSyntax syn:
                    ResolveFunction(syn);
                    break;
                case IClassDeclarationSyntax syn:
                    ResolveClass(syn);
                    break;
            }
        }

        private void ResolveFunction(IFunctionDeclarationSyntax function)
        {
            var analyzer = new TypeResolver(function.File, diagnostics);
            //ResolveTypesInParameters(analyzer, function.Parameters, null);
            //ResolveReturnType(function.ReturnDataType, function.ReturnType, analyzer);
        }

        private void ResolveClass(IClassDeclarationSyntax @class)
        {
            if (!@class.Symbol.TryBeginFulfilling(AddCircularDefinitionError))
                return;

            bool mutable = !(@class.MutableModifier is null);
            var classType = new ObjectType(@class.FullName, mutable, ReferenceCapability.Shared);

            var symbol = new TypeSymbol(@class.ContainingNamespaceSymbol!, @class.Name, classType);
            @class.Symbol.Fulfill(symbol);
            //@class.CreateDefaultConstructor();

            void AddCircularDefinitionError()
            {
                // TODO use something better than Name here which is an old name
                diagnostics.Add(TypeError.CircularDefinition(@class.File, @class.NameSpan, @class.Name.ToSimpleName()));
            }
        }
    }
}
