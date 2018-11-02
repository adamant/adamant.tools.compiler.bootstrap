using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class NameBuilder
    {
        [CanBeNull]
        public QualifiedName BuildName([NotNull] NameSyntax nameSyntax)
        {
            switch (nameSyntax)
            {
                case QualifiedNameSyntax qualifiedNameSyntax:
                    {
                        var qualifier = BuildName(qualifiedNameSyntax.Qualifier);
                        var name = qualifiedNameSyntax.Name.Name.Value;
                        return name != null ? qualifier?.Qualify(name) ?? new QualifiedName(name) : null;
                    }
                case IdentifierNameSyntax identifierNameSyntax:
                    {
                        var name = identifierNameSyntax.Name.Value;
                        return name != null ? new QualifiedName(name) : null;
                    }
                default:
                    throw NonExhaustiveMatchException.For(nameSyntax);
            }
        }
    }
}
