using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Lindhart.Analyser.MissingAwaitWarning;

namespace Lindhart.Analyser.MissingAwaitWarning.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

		//No diagnostics expected to show up
		[TestMethod]
		public void VerifyCode_EmptyCode_ExpectNoWarnings()
		{
			var test2 = @"";

			VerifyCSharpDiagnostic(test2);
		}

		//Diagnostic and CodeFix both triggered and checked for
		[TestMethod]
		public void VerifyCode_ProblematicCode_ExpectWarningForProblem()
		{

			var expected = new[] { new DiagnosticResult
						{
								Id = "LindhartAnalyserMissingAwaitWarning",
								Message = String.Format("The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited"),
								Severity = DiagnosticSeverity.Warning,
								Locations =
										new[] {
														new DiagnosticResultLocation("Test0.cs", 21, 13)
												}
						}, new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = String.Format("The method 'System.Threading.Tasks.Task<int>.ConfigureAwait(bool)' returns a Task that was not awaited"),
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[] {
														new DiagnosticResultLocation("Test0.cs", 26, 13)
												}
								}, };

			VerifyCSharpDiagnostic(TestData.TestDiagnosis, expected);
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
