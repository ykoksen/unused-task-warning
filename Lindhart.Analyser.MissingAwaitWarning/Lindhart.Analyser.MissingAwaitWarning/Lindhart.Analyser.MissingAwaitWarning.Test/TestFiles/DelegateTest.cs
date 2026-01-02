using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lindhart.Analyser.MissingAwaitWarning.Test.TestFiles
{
    internal class DelegateTest
    {
        public async Task TestDelegate(Func<int, Task> delegateReturningTask, Func<int> delegateNotReturningTask)
        {
            await delegateReturningTask(1);
            delegateNotReturningTask();            
        }
    }
}
