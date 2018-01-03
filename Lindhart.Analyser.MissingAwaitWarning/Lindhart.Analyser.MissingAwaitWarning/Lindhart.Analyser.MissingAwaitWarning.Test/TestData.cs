namespace Lindhart.Analyser.MissingAwaitWarning.Test
{
	internal class TestData
	{
		/// <summary>
		/// File for testing diagnosis
		/// </summary>
		public const string TestDiagnosis = @"
using System.Threading.Tasks;
namespace AsyncAwaitGames
{
    // In my real case, that method just returns Task.
    public interface ICallee { Task<int> DoSomethingAsync(); }

    public class Callee: ICallee
    {
        public async Task<int> DoSomethingAsync() => await Task.FromResult(0); // Should not give a warning
    }
    public class Caller
    {
        public void DoCall()
        {
            ICallee xxx = new Callee();

            // In my real case, the method just returns Task,
            // so there is no type mismatch when assigning a result 
            // either.
            xxx.DoSomethingAsync(); // Should give a warning.

            var task = xxx.DoSomethingAsync(); // Should not give a warning
            xxx.DoSomethingAsync().Result; // Should not give a warning

            xxx.DoSomethingAsync().ConfigureAwait(false); // Should give a warning
        }
    }
}";

		/// <summary>
		/// Input that should be fixed
		/// </summary>
		public const string FixTestInput = @"
using System.Threading.Tasks;
namespace AsyncAwaitGames
{
    // In my real case, that method just returns Task.
    public interface ICallee { Task<int> DoSomethingAsync(); }

    public class Callee: ICallee
    {
        public async Task<int> DoSomethingAsync() => await Task.FromResult(0); 
    }
    public class Caller
    {
        public void DoCall()
        {
            ICallee xxx = new Callee();

            #region test
            // In my real case, the method just returns Task,
            // so there is no type mismatch when assigning a result 
            // either.
            xxx.DoSomethingAsync(); // Should be fixed
            #endregion

            var task = xxx.DoSomethingAsync(); 
            xxx.DoSomethingAsync().Result; 
        }
    }
}";

		/// <summary>
		/// This is what we expect and hope the code will be fixed to
		/// </summary>
		public const string FixTestOutput = @"
using System.Threading.Tasks;
namespace AsyncAwaitGames
{
    // In my real case, that method just returns Task.
    public interface ICallee { Task<int> DoSomethingAsync(); }

    public class Callee: ICallee
    {
        public async Task<int> DoSomethingAsync() => await Task.FromResult(0); 
    }
    public class Caller
    {
        public void DoCall()
        {
            ICallee xxx = new Callee();

            #region test
            // In my real case, the method just returns Task,
            // so there is no type mismatch when assigning a result 
            // either.
            await xxx.DoSomethingAsync(); // Should be fixed
            #endregion

            var task = xxx.DoSomethingAsync(); 
            xxx.DoSomethingAsync().Result; 
        }
    }
}";

	}
}
