using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticTreeBuilder
    {
        private readonly SyntaxAnnotation<DataType> typeAnnotations;

        public SemanticTreeBuilder(SyntaxAnnotation<DataType> typeAnnotations)
        {
            this.typeAnnotations = typeAnnotations;
        }

        public CompilationUnit Build(SyntaxTree<CompilationUnitSyntax> syntaxTree)
        {
            var declarations = syntaxTree.Root.Declarations.Select(d => Build(d, typeAnnotations));
            return new CompilationUnit(syntaxTree.Root, declarations);
        }

        private Declaration Build(DeclarationSyntax declaration, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return MatchInto<Declaration>.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var access = BuildAccessModifier(f.AccessModifier, typeAnnotations);
                    var name = f.Name.Value;
                    var parameters = Build(f.Parameters, typeAnnotations);
                    var returnType = Build(f.ReturnType, typeAnnotations);
                    var body = Build(f.Body, typeAnnotations);
                    return new FunctionDeclaration(f, access, name, parameters, returnType, body);//TODO pass them into the constructor
                }));
        }

        private AccessLevel BuildAccessModifier(Token accessModifier, SyntaxAnnotation<DataType> typeAnnotations)
        {
            switch (accessModifier.Kind)
            {
                case TokenKind.PublicKeyword:
                    return AccessLevel.Public;
                default:
                    throw new NotSupportedException(accessModifier.ToString());
            }
        }

        private IEnumerable<Parameter> Build(ParameterListSyntax parameterList, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return parameterList.Parameters
                .Select(p => new Parameter(p, p.VarKeyword != null, p.Identifier.Value, Build(p.Type, typeAnnotations)))
                .ToList();
        }

        private TypeName Build(TypeSyntax type, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return new TypeName(type, typeAnnotations[type]);
        }

        private Block Build(BlockSyntax block, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return new Block(block);
        }
    }
}
