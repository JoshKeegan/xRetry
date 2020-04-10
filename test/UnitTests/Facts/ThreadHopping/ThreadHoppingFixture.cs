using System.Collections.Generic;
using System.Threading;
using Xunit.Abstractions;

namespace UnitTests.Facts.ThreadHopping
{
    public class ThreadHoppingFixture
    {
        public readonly int ThreadId = Thread.CurrentThread.ManagedThreadId;
        public readonly List<int> ThreadHistory = new List<int>();

        public readonly ExecutionContext ExecutionContext = Thread.CurrentThread.ExecutionContext;
        public readonly List<ExecutionContext> ExecutionContextHistory = new List<ExecutionContext>();

        public int NumAttempts = 0;

        public void AddAttempt()
        {
            NumAttempts++;
            ThreadHistory.Add(Thread.CurrentThread.ManagedThreadId);
            ExecutionContextHistory.Add(Thread.CurrentThread.ExecutionContext);
        }

        public void Assert(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.WriteLine($"Expected thread ID: {ThreadId}");
            foreach (int threadId in ThreadHistory)
            {
                testOutputHelper.WriteLine($"Thread ID: {threadId}");
                Xunit.Assert.Equal(ThreadId, threadId);
            }

            testOutputHelper.WriteLine($"Execution context type: {ExecutionContext.GetType()}");
            foreach (ExecutionContext ec in ExecutionContextHistory)
            {
                Xunit.Assert.Equal(ExecutionContext, ec);
            }
        }
    }
}
