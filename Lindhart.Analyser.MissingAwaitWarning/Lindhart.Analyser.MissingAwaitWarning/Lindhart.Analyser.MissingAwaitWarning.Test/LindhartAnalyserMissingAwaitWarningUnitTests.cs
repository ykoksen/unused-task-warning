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

			VerifyCSharpDiagnostic( test2 );
		}

		//Diagnostic and CodeFix both triggered and checked for
		[TestMethod]
		public void VerifyCode_ProblematicCode_ExpectWarningForProblem()
		{
			var expected = new[]
			{
                // Strict rule
                new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new []
												{
														new DiagnosticResultLocation("Test0.cs", 26, 24),
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new []
												{
														new DiagnosticResultLocation("Test0.cs", 27, 41),
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 28, 34)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new []
												{
														new DiagnosticResultLocation("Test0.cs", 29, 46),
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 30, 36)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 31, 37)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new []
												{
														new DiagnosticResultLocation("Test0.cs", 32, 47),
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarningStrict",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 33, 37)
												}
								},
                // Normal rule
                new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 36, 13)
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
														new DiagnosticResultLocation("Test0.cs", 37, 13)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 38, 32)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 39, 37)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 40, 39)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 43, 13)
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
														new DiagnosticResultLocation("Test0.cs", 45, 32),
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
														new DiagnosticResultLocation("Test0.cs", 49, 13)
												}
								},

								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 55, 13)
												}
								},
								new DiagnosticResult
								{
										Id = "LindhartAnalyserMissingAwaitWarning",
										Message = "The method 'AsyncAwaitGames.ICallee.DoSomethingAsync()' returns a Task that was not awaited",
										Severity = DiagnosticSeverity.Warning,
										Locations =
												new[]
												{
														new DiagnosticResultLocation("Test0.cs", 56, 13)
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
														new DiagnosticResultLocation("Test0.cs", 58, 33),
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
														new DiagnosticResultLocation("Test0.cs", 59, 33),
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
														new DiagnosticResultLocation("Test0.cs", 64, 13)
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
														new DiagnosticResultLocation("Test0.cs", 65, 13)
												}
								},
						};

			VerifyCSharpDiagnostic( TestData.TestDiagnosis, expected );
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

			VerifyCSharpDiagnostic( TestData.TestDiagnosisValueTask, expected );
		}

		[TestMethod]
		public void VerifyCodeFix_CodeFixApplied_CodeIsFixed()
		{
			VerifyCSharpFix( TestData.FixTestInput, TestData.FixTestOutput, allowNewCompilerDiagnostics: true );
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