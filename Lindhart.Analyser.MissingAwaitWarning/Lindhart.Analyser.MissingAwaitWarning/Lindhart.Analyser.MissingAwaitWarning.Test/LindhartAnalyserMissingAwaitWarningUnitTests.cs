using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Lindhart.Analyser.MissingAwaitWarning.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void VerifyCode_EmptyCode_ExpectNoWarnings()
        {
            string test2 = @"";

            VerifyCSharpDiagnostic(test2);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void VerifyCode_ProblematicCode_ExpectWarningForProblem()
        {
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarning",
                    Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 21, 13)
                        }
                },
                // Strict rule
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarningStrict",
                    Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = 
                        new []
                        {
                            new DiagnosticResultLocation("Test0.cs", 23, 24), 
                        }
                },
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarning",
                    Message = "The method 'System.Threading.Tasks.Task<int>.ConfigureAwait(bool)' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 26, 13)
                        }
                },

            };
            
            VerifyCSharpDiagnostic(TestData.TestDiagnosis, expected);
        }
        
        [TestMethod]
        public void VerifyCode_ProblematicCode_ExpectWarningForProblem_ValueTask()
        {
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarning",
                    Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 21, 13)
                        }
                },
                // Strict rule
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarningStrict",
                    Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = 
                        new []
                        {
                            new DiagnosticResultLocation("Test0.cs", 23, 24), 
                        }
                },
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarning",
                    Message = "The method 'System.Threading.Tasks.ValueTask<int>.ConfigureAwait(bool)' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 26, 13)
                        }
                },
            };

            VerifyCSharpDiagnostic(TestData.TestDiagnosisValueTask, expected);
        }

        [TestMethod]
        public void VerifyCode_ProblematicCode_ExpectWarningForProblem_ReturnTask()
        {
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = "LindhartAnalyserMissingAwaitWarningStrict",
                    Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 18, 20)
                        }
                }
            };

            VerifyCSharpDiagnostic(TestData.TestDiagnosisReturnTask, expected);
        }

        [TestMethod]
        public void VerifyCodeFix_CodeFixApplied_CodeIsFixed()
        {
            VerifyCSharpFix(TestData.FixTestInput, TestData.FixTestOutput, allowNewCompilerDiagnostics: true);
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