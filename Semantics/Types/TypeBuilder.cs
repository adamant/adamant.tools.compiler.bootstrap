using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeBuilder
    {
        private readonly Annotations annotations;

        internal TypeBuilder(Annotations annotations)
        {
            this.annotations = annotations;
        }

        /// This doesn't actually compute the types, rather it associates lazy values that when
        /// evaluated will compute the types
        public void BindBuilders(CompilationUnitSyntax compilationUnit)
        {
            foreach (var declaration in compilationUnit.Declarations)
                Bind(declaration);
        }

        #region Bind builders
        private void Bind(DeclarationSyntax declaration)
        {
            Match.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    AddTypeAnnotation(f, GetType);
                    foreach (var parameter in f.ParameterList.Parameters)
                        BindParameter(parameter);
                    Bind(f.ReturnType);
                })
            // TODO varible declarations syntax needs checked
            // TODO for loop syntax needs checked
            // TODO if let syntax needs checked
            );
        }

        private void BindParameter(ParameterSyntax parameter)
        {
            Bind(parameter.Type);
            AddTypeAnnotation(parameter, GetType);
        }

        private void Bind(TypeSyntax type)
        {
            Match.On(type).With(m => m
               .Is<PrimitiveTypeSyntax>(p =>
               {
                   AddTypeAnnotation(p, PrimitiveType.New(p.Keyword.Kind));
               }));
        }
        #endregion

        #region AddTypeAnnotation
        private void AddTypeAnnotation<T>(T syntax, Func<T, DataType> getType)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<DataType>(() => getType(syntax)));
        }

        private void AddTypeAnnotation<T>(T syntax, DataType type)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<DataType>(type));
        }
        #endregion

        #region Get Type
        private DataType GetType(FunctionDeclarationSyntax function)
        {
            var parameterTypes = function.ParameterList.Parameters.Select(p => annotations.Type(p));
            var returnType = annotations.Type(function.ReturnType);
            return new FunctionType(parameterTypes, returnType);
        }

        private DataType GetType(ParameterSyntax parameter)
        {
            return annotations.Type(parameter.Type);
        }
        #endregion
    }
}
