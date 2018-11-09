using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class NameBuilder
    {
        [CanBeNull]
        public Name BuildName([NotNull] NameSyntax nameSyntax)
        {
            switch (nameSyntax)
            {
                case QualifiedNameSyntax qualifiedNameSyntax:
                {
                    var qualifier = BuildName(qualifiedNameSyntax.Qualifier);
                    var name = qualifiedNameSyntax.Name.Name.Value;
                    return name != null ? qualifier?.Qualify(name) ?? new SimpleName(name) : null;
                }
                case IdentifierNameSyntax identifierNameSyntax:
                {
                    var name = identifierNameSyntax.Name.Value;
                    return name != null ? new SimpleName(name) : null;
                }
                default:
                    throw NonExhaustiveMatchException.For(nameSyntax);
            }
        }
    }
}
