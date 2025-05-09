using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace UnitTests.v3.Facts.ThreadHopping
{
    public class ThreadHoppingFixture
    {
        public readonly int ThreadId = Thread.CurrentThread.ManagedThreadId;
        public readonly List<int> ThreadHistory = new List<int>();

        public int NumAttempts = 0;

        public void AddAttempt()
        {
            NumAttempts++;
            ThreadHistory.Add(Thread.CurrentThread.ManagedThreadId);
        }

        public void Assert(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.WriteLine($"Expected thread ID: {ThreadId}");
            foreach (int threadId in ThreadHistory)
            {
                testOutputHelper.WriteLine($"Thread ID: {threadId}");
                threadId.Should().Be(ThreadId);
            }
        }
    }
}
