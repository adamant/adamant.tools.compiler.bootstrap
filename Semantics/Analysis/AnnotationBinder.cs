using System;
using System.Linq;
using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class AnnotationBinder
    {
        private readonly Annotations annotations;
        private readonly TypeChecker typeChecker;
        private readonly NameBinder nameBinder;

        internal AnnotationBinder(Annotations annotations)
        {
            this.annotations = annotations;
            typeChecker = new TypeChecker(annotations);
            nameBinder = new NameBinder(annotations);
        }

        /// This doesn't actually compute the types etc, rather it associates
        /// lazy values that when evaluated will compute the types etc.
        public void Bind(PackageSyntax package)
        {
            AnnotateName(package, nameBinder.NameOf(package));
            Parallel.ForEach(package.SyntaxTrees.Select(t => t.Root), Bind);
        }

        private void Bind(CompilationUnitSyntax compilationUnit)
        {
            AnnotateName(compilationUnit, nameBinder.NameOf(compilationUnit));
            foreach (var declaration in compilationUnit.Declarations)
                Bind(declaration);
        }

        #region Bind builders
        private void Bind(DeclarationSyntax declaration)
        {
            Match.On(declaration).With(m => m
                    .Is<FunctionDeclarationSyntax>(f =>
                    {
                        AnnotateType(f, typeChecker.GetType);
                        AnnotateName(f, nameBinder.GetName);
                        foreach (var parameter in f.ParameterList.Parameters)
                            BindParameter(parameter);
                        Bind(f.ReturnType);
                        Bind(f.Body);
                    })
            // TODO varible declarations syntax needs checked
            // TODO for loop syntax needs checked
            // TODO if let syntax needs checked
            );
        }

        private void Bind(BlockSyntax block)
        {
            foreach (var statement in block.Statements)
                Bind(statement);
        }

        private void Bind(StatementSyntax statement)
        {
            Match.On(statement).With(m => m
                .Is<ExpressionStatementSyntax>(es => Bind(es.Expression)));
        }

        private void Bind(ExpressionSyntax expression)
        {
            Match.On(expression).With(m => m
                .Is<ReturnExpressionSyntax>(r =>
                {
                    if (r.Expression != null)
                        Bind(r.Expression);
                })
                .Is<BinaryOperatorExpressionSyntax>(binOp =>
                {
                    Bind(binOp.LeftOperand);
                    Bind(binOp.RightOperand);
                })
                .Is<IdentifierNameSyntax>(n =>
                {
                    AnnotateName(n, nameBinder.GetName);
                    AnnotateType(n, typeChecker.GetType);
                }));
        }

        private void BindParameter(ParameterSyntax parameter)
        {
            Bind(parameter.Type);
            AnnotateName(parameter, nameBinder.GetName);
            AnnotateType(parameter, typeChecker.GetType);
        }

        private void Bind(TypeSyntax type)
        {
            Match.On(type).With(m => m
                .Is<PrimitiveTypeSyntax>(p =>
                {
                    AnnotateType(p, typeChecker.TypeOf(p));
                }));
        }
        #endregion

        #region AnnotateType
        private void AnnotateType<T>(T syntax, Func<T, DataType> getType)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<DataType>(() => getType(syntax)));
        }

        private void AnnotateType<T>(T syntax, DataType type)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<DataType>(type));
        }

        private void AnnotateName<T>(T syntax, Func<T, Name> getName)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<Name>(() => getName(syntax)));
        }

        private void AnnotateName<T>(T syntax, Name name)
            where T : SyntaxBranchNode
        {
            annotations.Add(syntax, new Lazy<Name>(name));
        }
        #endregion
    }
}
