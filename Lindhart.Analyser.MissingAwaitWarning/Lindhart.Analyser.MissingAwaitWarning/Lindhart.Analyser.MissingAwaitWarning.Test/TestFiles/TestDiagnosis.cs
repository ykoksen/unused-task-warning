
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
            await (xxx?.DoSomethingAsync() ?? Task.FromResult(0)); // Should not give a warning           
            await (holder?.Callee?.DoSomethingAsync() ?? Task.FromResult(0)); // Should not give a warning
            await (holder?.Callee.DoSomethingAsync() ?? Task.FromResult(0)); // Should not give a warning             
        }
    }
    public class CallerHolder
    {
        public ICallee Callee { get; set; }
    }
}
