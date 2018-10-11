using Adamant.Tools.Compiler.Bootstrap.Semantics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class PackageEmitter : IEmitter<Package>
    {
        [NotNull] private readonly NameMangler nameMangler;
        [NotNull] private readonly IEmitter<Declaration> declarationEmitter;

        public PackageEmitter(
            [NotNull] NameMangler nameMangler,
            [NotNull] IEmitter<Declaration> declarationEmitter)
        {
            this.nameMangler = nameMangler;
            this.declarationEmitter = declarationEmitter;
        }

        public void Emit([NotNull] Package package, [NotNull] Code code)
        {
            EmitPreamble(code);

            foreach (var declaration in package.Declarations)
                declarationEmitter.Emit(declaration, code);

            EmitEntryPointAdapter(package, code);

            EmitPostamble(code);
        }

        private static void EmitPreamble([NotNull] Code code)
        {
            // Setup the beginning of each section
            code.Includes.AppendLine($"#include \"{CodeEmitter.RuntimeLibraryHeaderFileName}\"");

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

        private void EmitEntryPointAdapter([NotNull] Package package, [NotNull] Code code)
        {
            if (package.EntryPoint == null) return;

            var entryPoint = package.EntryPoint;
            code.Definitions.DeclarationSeparatorLine();
            code.Definitions.AppendLine("// Entry Point Adapter");
            code.Definitions.AppendLine("int32_t main(const int argc, char const * const * const argv)");
            code.Definitions.BeginBlock();
            code.Definitions.AppendLine($"{nameMangler.MangleName(entryPoint)}();");
            code.Definitions.AppendLine("return 0;");
            code.Definitions.EndBlock();
        }

        private static void EmitPostamble([NotNull] Code code)
        {
            // Close the Type_ID enum
            //code.TypeIdDeclaration.EndBlockWithSemicolon();
            //code.TypeIdDeclaration.AppendLine("typedef enum Type_ID Type_ID;");
        }
    }
}
