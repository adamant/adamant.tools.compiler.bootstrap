using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class NameBinder
    {
        private readonly Annotations annotations;
        private PackageName packageName;
        private GlobalNamespaceName globalNamespaceName;

        public NameBinder(Annotations annotations)
        {
            this.annotations = annotations;
        }

        public Name NameOf(PackageSyntax package)
        {
            // TODO get a real package name
            packageName = new PackageName("default");
            globalNamespaceName = new GlobalNamespaceName(packageName);
            return packageName;
        }

        public Name NameOf(CompilationUnitSyntax compilationUnit)
        {
            return globalNamespaceName;
        }

        public Name GetName(FunctionDeclarationSyntax functionDeclaration)
        {
            var scope = annotations.Scope(functionDeclaration);
            var parentSyntax = scope.Parent.Symbol.Declarations.Single();
            var parentName = (ScopeName)annotations.Name(parentSyntax);
            return new FunctionName(parentName, functionDeclaration.Name.Value, functionDeclaration.ParameterList.Parameters.Count());
        }

        public Name GetName(ParameterSyntax parameter)
        {
            var scope = annotations.Scope(parameter);
            var functionSyntax = scope.Symbol.Declarations.Single();
            var parentName = (ScopeName)annotations.Name(functionSyntax);
            return new VariableName(parentName, parameter.Identifier.Text);
        }

        public Name GetName(IdentifierNameSyntax identifierName)
        {
            var nameScope = annotations.Scope(identifierName);
            var symbol = nameScope.LookupName(identifierName.Identifier.Text);
            var declaration = symbol.Declarations.Single();
            return annotations.Name(declaration);
        }
    }
}
