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
    public interface ICallee { Task<int> DoSomethingAsync(); }

    public class Callee : ICallee
    {
        public async Task<int> DoSomethingAsync() => await Task.FromResult(0);
    }

    public class Caller
    {
        public async Task DoCall()
        {
            ICallee xxx = new Callee();

            // Should not give a warning
            await xxx.DoSomethingAsync();
            xxx.DoSomethingAsync().Result;
            xxx.DoSomethingAsync().Wait();

            // Should give a warning when strict rule enabled
            var task = xxx.DoSomethingAsync();
            var LocalFunc() => { var _ = xxx.DoSomethingAsync(); };
            Action action = () => { var _ = xxx.DoSomethingAsync(); };
            Parallel.For(0, 5, i => { var _ = xxx.DoSomethingAsync(); });

            // Should always give a warning
            xxx.DoSomethingAsync();
            xxx.DoSomethingAsync().ConfigureAwait(false);
            var LocalFunc1() => xxx.DoSomethingAsync();
            var LocalFunc2() => { xxx.DoSomethingAsync(); };
            Action action1 = () => xxx.DoSomethingAsync();
            Action action2 = () => { xxx.DoSomethingAsync(); };
            Parallel.For(0, 5, i => xxx.DoSomethingAsync());
            Parallel.For(0, 5, i => { xxx.DoSomethingAsync(); });
    }
}";

        /// <summary>
        /// File for testing diagnosis
        /// </summary>
        public const string TestDiagnosisValueTask = @"
using System.Threading.Tasks;
namespace AsyncAwaitGames
{
    // In my real case, that method just returns Task.
    public interface ICallee { ValueTask<int> DoSomethingAsync(); }

    public class Callee: ICallee
    {
        public async ValueTask<int> DoSomethingAsync() => await Task.FromResult(0); // Should not give a warning
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

            var task = xxx.DoSomethingAsync(); // Should give a warning when strict rule enabled
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

            var task = await xxx.DoSomethingAsync();
            xxx.DoSomethingAsync().Result;
        }
    }
}";
    }
}
