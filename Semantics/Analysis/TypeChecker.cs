using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class TypeChecker
    {
        private readonly Annotations annotations;
        private readonly IPackageSyntaxSymbol packageSymbol;

        public TypeChecker(Annotations annotations)
        {
            this.annotations = annotations;
            packageSymbol = annotations.Symbol(annotations.Package);
        }

        public DataType GetType(FunctionDeclarationSyntax function)
        {
            var parameterTypes = function.ParameterList.Parameters.Select(p => annotations.Type(p));
            var returnType = annotations.Type(function.ReturnType);
            return new FunctionType(parameterTypes, returnType);
        }

        public DataType GetType(ParameterSyntax parameter)
        {
            return annotations.Type(parameter.Type);
        }

        public PrimitiveType TypeOf(PrimitiveTypeSyntax primitiveType)
        {
            return PrimitiveType.New(primitiveType.Keyword.Kind);
        }

        public DataType GetType(IdentifierNameSyntax identifierName)
        {
            var variableName = (VariableName)annotations.Name(identifierName);
            var symbol = packageSymbol.Lookup(variableName);
            var syntax = symbol.Declarations.Single();
            return annotations.Type(syntax);
        }
    }
}
