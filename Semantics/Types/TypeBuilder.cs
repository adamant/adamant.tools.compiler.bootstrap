using System;
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
                }));
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
                   AddTypeAnnotation(p, GetType);
               }));
        }
        #endregion

        private void AddTypeAnnotation<T>(T syntax, Func<T, DataType> getType)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<DataType>(() => getType(syntax)));
        }

        #region Get Type
        private DataType GetType(FunctionDeclarationSyntax functionDeclaration)
        {
            throw new NotImplementedException();
        }

        private DataType GetType(ParameterSyntax parameter)
        {
            throw new NotImplementedException();
        }

        private DataType GetType(PrimitiveTypeSyntax primitiveType)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
