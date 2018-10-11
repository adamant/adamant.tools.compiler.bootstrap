using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class Code
    {
        [NotNull] public readonly CCodeBuilder Includes = new CCodeBuilder();
        [NotNull] public readonly CCodeBuilder TypeIdDeclaration = new CCodeBuilder();
        [NotNull] public readonly CCodeBuilder TypeDeclarations = new CCodeBuilder();
        [NotNull] public readonly CCodeBuilder FunctionDeclarations = new CCodeBuilder();
        [NotNull] public readonly CCodeBuilder ClassDeclarations = new CCodeBuilder();
        [NotNull] public readonly CCodeBuilder GlobalDefinitions = new CCodeBuilder();
        [NotNull] public readonly CCodeBuilder Definitions = new CCodeBuilder();

        public override string ToString()
        {
            return string.Concat(
                Includes.Code,
                CCodeBuilder.LineTerminator,
                TypeIdDeclaration.Code,
                CCodeBuilder.LineTerminator,
                TypeDeclarations.Code,
                CCodeBuilder.LineTerminator,
                FunctionDeclarations.Code,
                CCodeBuilder.LineTerminator,
                ClassDeclarations.Code,
                CCodeBuilder.LineTerminator,
                GlobalDefinitions.Code,
                CCodeBuilder.LineTerminator,
                Definitions.Code);
        }
    }
}
