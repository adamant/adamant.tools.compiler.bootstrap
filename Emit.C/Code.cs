namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class Code
    {
        public readonly CCodeBuilder Includes = new CCodeBuilder();
        public readonly CCodeBuilder TypeIdDeclaration = new CCodeBuilder();
        public readonly CCodeBuilder TypeDeclarations = new CCodeBuilder();
        public readonly CCodeBuilder FunctionDeclarations = new CCodeBuilder();
        public readonly CCodeBuilder StructDeclarations = new CCodeBuilder();
        public readonly CCodeBuilder GlobalDefinitions = new CCodeBuilder();
        public readonly CCodeBuilder Definitions = new CCodeBuilder();

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
                StructDeclarations.Code,
                CCodeBuilder.LineTerminator,
                GlobalDefinitions.Code,
                CCodeBuilder.LineTerminator,
                Definitions.Code);
        }
    }
}
