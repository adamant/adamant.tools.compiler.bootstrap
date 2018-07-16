using System.Xml.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class ParseTestCase
    {
        public CodePath CodePath { get; }
        public string Code { get; }
        public ParseTestSyntaxKind SyntaxKind { get; }
        public XElement RootSyntax { get; }

        public ParseTestCase(CodePath codePath, string code, ParseTestSyntaxKind syntaxKind, XElement rootSyntax)
        {
            CodePath = codePath;
            Code = code;
            SyntaxKind = syntaxKind;
            RootSyntax = rootSyntax;
        }
    }
}
