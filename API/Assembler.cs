using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class Assembler
    {
        public string Disassemble(Package package)
        {
            var builder = new AssemblyBuilder();
            foreach (var declaration in package.Declarations)
            {
                Disassemble(declaration, builder);
                builder.BlankLine();
            }

            return builder.Code;
        }

        public void Disassemble(Declaration declaration, AssemblyBuilder builder)
        {
            switch (declaration)
            {
                case FunctionDeclaration function:
                    Disassemble(function, builder);
                    break;
                case ConstructorDeclaration constructor:
                    Disassemble(constructor, builder);
                    break;
                case TypeDeclaration type:
                    Disassemble(type, builder);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        public void Disassemble(FunctionDeclaration function, AssemblyBuilder builder)
        {
            var parameters = string.Join(", ", function.Parameters.Select(FormatParameter));
            builder.BeginLine("fn ");
            builder.Append(function.FullName.ToString());
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine("\t=> ");
            builder.EndLine(function.ReturnType.ToString());
            builder.BeginBlock();
            builder.EndBlock();
        }

        private static string FormatParameter(Parameter parameter)
        {
            var format = parameter.MutableBinding ? "mut {0}: {1}" : "{0}: {1}";
            return string.Format(format, parameter.Name, parameter.Type);
        }

        public void Disassemble(ConstructorDeclaration constructor, AssemblyBuilder builder)
        {
            var parameters = string.Join(", ", constructor.Parameters.Select(FormatParameter));
            builder.BeginLine("fn ");
            builder.Append(constructor.FullName.ToString());
            builder.Append("(");
            builder.Append(parameters);
            builder.Append(")");
            builder.EndLine();
            builder.BeginLine("\t=> ");
            builder.EndLine(constructor.ReturnType.ToString());
            builder.BeginBlock();
            builder.EndBlock();
        }

        public void Disassemble(TypeDeclaration type, AssemblyBuilder builder)
        {
            builder.BeginLine("type ");
            builder.Append(type.FullName.ToString());
            builder.EndLine();
            builder.BeginBlock();
            foreach (var member in type.Members)
            {
                builder.DeclarationSeparatorLine();
                Disassemble(member, builder);
            }
            builder.EndBlock();
        }
    }
}
