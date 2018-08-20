namespace Adamant.Tools.Compiler.Bootstrap.IL.Declarations
{
    public class TypeDeclaration : Declaration
    {
        public readonly string Name;
        public readonly bool IsReference;

        public TypeDeclaration(string name, bool isReference)
        {
            Name = name;
            IsReference = isReference;
        }

        internal override void ToString(AsmBuilder builder)
        {
            var kind = IsReference ? "class" : "struct";
            builder.AppendLine($"public {kind} {Name}");
            builder.BeginBlock();
            builder.EndBlock();
        }
    }
}
