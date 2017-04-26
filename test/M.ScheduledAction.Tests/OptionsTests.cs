using System;
using System.Threading;
using FakeItEasy;
using Xunit;

namespace M.ScheduledAction.Tests
{
    public class OptionsTests
    {
        public interface IHandler
        {
            void OnError(IScheduledAction scheduledAction, Exception exception);
            void OnReschedule(IScheduledAction scheduledAction, TimeSpan interval);
        }

        [Fact]
        public void New_ExecuteOnStart_IsFalse()
        {
            var options = new Options();

            Assert.False(options.ExecuteOnStart);
        }

        [Fact]
        public void InvokeOnError_OnErrorIsNotSet_HasNoEffect()
        {
            var scheduledAction = A.Fake<IScheduledAction>();
            var options = new Options();

            options.InvokeOnError(scheduledAction, new Exception());

            Assert.True(true);
        }

        [Fact]
        public void InvokeOnError_OnErrorIsSet_Called()
        {
            var scheduledAction = A.Fake<IScheduledAction>();
            var exception = new Exception();
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnError = handler.OnError };

            options.InvokeOnError(scheduledAction, exception);

            Thread.Sleep(50);
            A.CallTo(() => handler.OnError(scheduledAction, exception))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void InvokeOnReschedule_OnErrorIsNotSet_HasNoEffect()
        {
            var scheduledAction = A.Fake<IScheduledAction>();
            var options = new Options();

            options.InvokeOnReschedule(scheduledAction, TimeSpan.Zero);

            Assert.True(true);
        }

        [Fact]
        public void InvokeOnReschedule_RescheduleIsSet_Called()
        {
            var scheduledAction = A.Fake<IScheduledAction>();
            var interval = TimeSpan.MaxValue;
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnReschedule = handler.OnReschedule };

            options.InvokeOnReschedule(scheduledAction, interval);

            Thread.Sleep(50);
            A.CallTo(() => handler.OnReschedule(scheduledAction, interval))
             .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
