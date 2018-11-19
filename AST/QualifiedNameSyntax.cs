using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class QualifiedNameSyntax : NameSyntax
    {
        [NotNull] public NameSyntax Qualifier { get; }
        [NotNull] public SimpleNameSyntax Name { get; }

        public QualifiedNameSyntax([NotNull] NameSyntax qualifier, [NotNull] SimpleNameSyntax name)
            : base(TextSpan.Covering(qualifier.Span, name.Span))
        {
            Qualifier = qualifier;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Qualifier}.{Name}";
        }
    }
}
