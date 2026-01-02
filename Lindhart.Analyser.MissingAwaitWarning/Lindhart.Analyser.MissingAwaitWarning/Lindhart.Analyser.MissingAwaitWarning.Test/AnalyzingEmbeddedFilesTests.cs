using Lindhart.Analyser.MissingAwaitWarning.Test.Helpers;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TestHelper;

namespace Lindhart.Analyser.MissingAwaitWarning.Test
{
    [TestClass]
    public class AnalyzingEmbeddedFilesTests : CodeFixVerifier
    {
        [TestMethod]
        public async Task Given_CodeWithCorrectDelegates_When_AnalyzingEmbeddedFiles_Then_NoWarningOrErrorsAreReported()
        {
            VerifyCSharpDiagnostic(await EmbeddedFilesLoader.LoadFile("DelegateTest"));
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new LindhartAnalyserMissingAwaitWarningCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new LindhartAnalyserMissingAwaitWarningAnalyzer();
        }
    }
}
