namespace Lindhart.Analyser.MissingAwaitWarning.Test
{
    internal class TestData
    {
        /// <summary>
        /// File for testing diagnosis
        /// </summary>
        public const string TestDiagnosis = @"
using System;
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
            var __ = xxx.DoSomethingAsync().Result;
            xxx.DoSomethingAsync().Wait();
            Func<Task> ___ = async () => await Task.Delay(100);

            // Should give a warning when strict rule enabled
            var task = xxx.DoSomethingAsync();
            void LocalFunc1() { var _ = xxx.DoSomethingAsync(); };
            void LocalFunc2() => xxx.DoSomethingAsync();
            Action action1 = () => { var _ = xxx.DoSomethingAsync(); };
            Action action2 = () => xxx.DoSomethingAsync();
            Func<Task> func = () => xxx.DoSomethingAsync();
            Parallel.For(0, 5, i => { var _ = xxx.DoSomethingAsync(); });
            Parallel.For(0, 5, i => xxx.DoSomethingAsync());

            // Should always give a warning
            xxx.DoSomethingAsync();
            xxx.DoSomethingAsync().ConfigureAwait(false);
            void LocalFunc() { xxx.DoSomethingAsync(); };
            Action action = () => { xxx.DoSomethingAsync(); };
            Parallel.For(0, 5, i => { xxx.DoSomethingAsync(); });

            // Null Conditional                                                                                                   
            xxx?.DoSomethingAsync(); // Should give a warning                                                                     
                                                                                                                                  
            var taskNullable = xxx?.DoSomethingAsync(); // Should give a warning when strict rule enabled                         
                                                                                                                                  
            xxx?.DoSomethingAsync().Result; // Should not give a warning                                                          
                                                                                                                                  
            xxx?.DoSomethingAsync().ConfigureAwait(false); // Should give a warning                                               
                                                                                                                                  
            var holder = new CallerHolder();                                                                                      
                                                                                                                                  
                                                                                                                                  
            // Null Conditional second level                                                                                      
            holder?.Callee?.DoSomethingAsync(); // Should give a warning                                                          
            holder?.Callee.DoSomethingAsync(); // Should give a warning                                                           
                                                                                                                                  
            var taskNullable2 = holder?.Callee?.DoSomethingAsync(); // Should give a warning when strict rule enabled             
            var taskNullable3 = holder?.Callee.DoSomethingAsync(); // Should give a warning when strict rule enabled              
                                                                                                                                  
            holder?.Callee?.DoSomethingAsync().Result; // Should not give a warning                                               
            holder?.Callee.DoSomethingAsync().Result; // Should not give a warning                                                
                                                                                                                                  
            holder?.Callee?.DoSomethingAsync().ConfigureAwait(false); // Should give a warning                                    
            holder?.Callee.DoSomethingAsync().ConfigureAwait(false); // Should give a warning                                     
                                                                                                                                  
                                                                                                                                  
                                                                                                                                  
            // Awaited                                                                                                            
            await xxx.DoSomethingAsync(); // Should not give a warning                                                            
            await xxx?.DoSomethingAsync(); // Should not give a warning                                                           
            await holder?.Callee?.DoSomethingAsync(); // Should not give a warning                                                
            await holder?.Callee.DoSomethingAsync(); // Should not give a warning 
            await ( xxx?.DoSomethingAsync() ?? Task.FromResult(0) ); // Should not give a warning           
            await ( holder?.Callee?.DoSomethingAsync() ?? Task.FromResult(0) ); // Should not give a warning
            await ( holder?.Callee.DoSomethingAsync() ?? Task.FromResult(0) ); // Should not give a warning             
        }
    }
    public class CallerHolder                                                                                                              
    {                                                                                                                                      
        public ICallee Callee {get;set;}                                                                                                   
    }
}";

        /// <summary>
        /// This should give no warnings
        /// </summary>
        public const string TestLocalFunctions = @"
using System;
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
                
                // Functions that should not give warning
                DoSomething(() => xxx.DoSomethingAsync());
                Func<Func<Task>> func = () => () => xxx.DoSomethingAsync();
            }

            public void DoSomething(Func<Task> action){}

            public Task TestThis(ICallee test) => test.DoSomethingAsync());
        }
        public class CallerHolder
        {
            public ICallee Callee { get; set; }
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
                                                                                                                                  
                                                                                                                                  
            // Null Conditional                                                                                                   
            xxx?.DoSomethingAsync(); // Should give a warning                                                                     
                                                                                                                                  
            var taskNullable = xxx?.DoSomethingAsync(); // Should give a warning when strict rule enabled                         
                                                                                                                                  
            xxx?.DoSomethingAsync().Result; // Should not give a warning                                                          
                                                                                                                                  
            xxx?.DoSomethingAsync().ConfigureAwait(false); // Should give a warning                                               
                                                                                                                                  
            var holder = new CallerHolder();                                                                                      
                                                                                                                                  
                                                                                                                                  
            // Null Conditional second level                                                                                      
            holder?.Callee?.DoSomethingAsync(); // Should give a warning                                                          
            holder?.Callee.DoSomethingAsync(); // Should give a warning                                                           
                                                                                                                                  
            var taskNullable2 = holder?.Callee?.DoSomethingAsync(); // Should give a warning when strict rule enabled             
            var taskNullable3 = holder?.Callee.DoSomethingAsync(); // Should give a warning when strict rule enabled              
                                                                                                                                  
            holder?.Callee?.DoSomethingAsync().Result; // Should not give a warning                                               
            holder?.Callee.DoSomethingAsync().Result; // Should not give a warning                                                
                                                                                                                                  
            holder?.Callee?.DoSomethingAsync().ConfigureAwait(false); // Should give a warning                                    
            holder?.Callee.DoSomethingAsync().ConfigureAwait(false); // Should give a warning                                     
                                                                                                                                  
                                                                                                                                  
                                                                                                                                  
            // Awaited                                                                                                            
            await xxx.DoSomethingAsync(); // Should not give a warning                                                            
            await xxx?.DoSomethingAsync(); // Should not give a warning                                                           
            await holder?.Callee?.DoSomethingAsync(); // Should not give a warning                                                
            await holder?.Callee.DoSomethingAsync(); // Should not give a warning 
            await ( xxx?.DoSomethingAsync() ?? Task.FromResult(0) ); // Should not give a warning           
            await ( holder?.Callee?.DoSomethingAsync() ?? Task.FromResult(0) ); // Should not give a warning
            await ( holder?.Callee.DoSomethingAsync() ?? Task.FromResult(0) ); // Should not give a warning
        }
    }
    public class CallerHolder                                                                                                              
    {                                                                                                                                      
        public ICallee Callee {get;set;}    
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

        /// <summary>
        /// Input that should be fixed
        /// </summary>
        public const string AutogeneratedCode = @"
//------------------------------------------------------------------------------
// <auto-generated>
//	This code was generated by a tool. DO NOT EDIT
// </auto-generated>
//------------------------------------------------------------------------------
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
    }
}
