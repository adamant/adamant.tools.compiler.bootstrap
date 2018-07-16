using System.Xml.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class ParseTestCase
    {
        public string Code { get; }
        public ParseTestSyntaxKind SyntaxKind { get; }
        public XElement RootSyntax { get; }

        public ParseTestCase(string code, ParseTestSyntaxKind syntaxKind, XElement rootSyntax)
        {
            this.Code = code;
            this.SyntaxKind = syntaxKind;
            this.RootSyntax = rootSyntax;
        }
    }
}
