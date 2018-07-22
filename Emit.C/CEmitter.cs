using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CEmitter : Emitter
    {
        public override string Emit(Package package)
        {
            var code = new Code();

            EmitPreamble(code);

            foreach (var compilationUnit in package.CompilationUnits)
                Emit(code, compilationUnit);

            EmitEntryPointAdapter(code, package);

            EmitPostamble(code);

            return code.ToString();
        }

        internal void EmitPreamble(Code code)
        {
            // Setup the beginning of each section
            code.Includes.AppendLine("#include \"RuntimeLibrary.h\"");

            code.TypeIdDeclaration.AppendLine("// Type ID Declarations");
            code.TypeIdDeclaration.AppendLine("enum Type_ID");
            code.TypeIdDeclaration.BeginBlock();
            // TODO setup primitive types?

            code.TypeDeclarations.AppendLine("// Type Declarations");
            code.FunctionDeclarations.AppendLine("// Function Declarations");
            code.ClassDeclarations.AppendLine("// Class Declarations");
            code.GlobalDefinitions.AppendLine("// Global Definitions");
            code.Definitions.AppendLine("// Definitions");
        }

        internal void Emit(Code code, CompilationUnit compilationUnit)
        {
            // TODO implement
        }

        internal void EmitEntryPointAdapter(Code code, Package package)
        {
            // TODO implement
        }

        internal void EmitPostamble(Code code)
        {
            // Close the Type_ID enum
            code.TypeIdDeclaration.EndBlockWithSemicolon();
            code.TypeIdDeclaration.AppendLine("typedef enum Type_ID Type_ID;");
        }
    }
}
