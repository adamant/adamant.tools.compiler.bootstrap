using Adamant.Tools.Compiler.Bootstrap.Emit.C.Properties;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CodeEmitter : Emitter
    {
        [NotNull] private readonly NameMangler nameMangler = new NameMangler();

        [NotNull] public static string RuntimeLibraryCode => Resources.RuntimeLibraryCode.AssertNotNull();
        [NotNull] public const string RuntimeLibraryCodeFileName = "RuntimeLibrary.c";
        [NotNull] public static string RuntimeLibraryHeader => Resources.RuntimeLibraryHeader.AssertNotNull();
        [NotNull] public const string RuntimeLibraryHeaderFileName = "RuntimeLibrary.h";

        public override string Emit([NotNull] Package package)
        {
            var code = new Code();

            EmitPreamble(code);

            foreach (var declaration in package.Declarations)
                Emit(declaration, code);

            EmitEntryPointAdapter(code, package);

            EmitPostamble(code);

            return code.ToString();
        }

        private void EmitPreamble([NotNull] Code code)
        {
            // Setup the beginning of each section
            code.Includes.AppendLine($"#include \"{RuntimeLibraryHeaderFileName}\"");

            code.TypeIdDeclaration.AppendLine("// Type ID Declarations");
            // Type ID Enum
            //code.TypeIdDeclaration.AppendLine("enum Type_ID");
            //code.TypeIdDeclaration.BeginBlock();
            //code.TypeIdDeclaration.AppendLine("never__0__0TypeID = 0,");
            // TODO setup primitive types?

            code.TypeDeclarations.AppendLine("// Type Declarations");
            code.FunctionDeclarations.AppendLine("// Function Declarations");
            code.ClassDeclarations.AppendLine("// Class Declarations");
            code.GlobalDefinitions.AppendLine("// Global Definitions");
            code.Definitions.AppendLine("// Definitions");
        }

        private void Emit([NotNull] Declaration declaration, [NotNull]  Code code)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    Emit(function, code);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void Emit([NotNull] FunctionDeclaration function, [NotNull] Code code)
        {

        }

        private void EmitEntryPointAdapter([NotNull] Code code, [NotNull] Package package)
        {
        }

        private void EmitPostamble([NotNull] Code code)
        {
            // Close the Type_ID enum
            //code.TypeIdDeclaration.EndBlockWithSemicolon();
            //code.TypeIdDeclaration.AppendLine("typedef enum Type_ID Type_ID;");
        }
    }
}
