using System;
using System.Threading;
using FakeItEasy;
using Xunit;

namespace M.ScheduledAction.Tests
{
    public class ScheduledActionTests
    {
        public interface IAction
        {
            void DoSomething();
        }

        public interface IHandler
        {
            void OnError(IScheduledAction scheduledAction, Exception exception);
            void OnReschedule(IScheduledAction scheduledAction, TimeSpan interval);
        }

        [Fact]
        public void New_WithActionNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ScheduledAction(null, A.Fake<ISchedule>()));
        }

        [Fact]
        public void New_WithScheduleNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ScheduledAction(()=> { }, null));
        }

        [Fact]
        public void Start_AlreadyStarted_Throws()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter()).Returns(TimeSpan.FromSeconds(1));
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule);

            scheduledAction.Start();

            Assert.Throws<InvalidOperationException>(() => scheduledAction.Start());
        }

        [Fact]
        public void Start_WithOptionExecuteOnStart_ActionCalledOnStart()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter()).Returns(TimeSpan.FromMilliseconds(-1));
            var options = new Options() { ExecuteOnStart = true };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => action.DoSomething())
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Start_WithOptionOnReschedule_OnRescheduleCalled()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnReschedule = handler.OnReschedule };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => handler.OnReschedule(A<IScheduledAction>.Ignored, A<TimeSpan>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void Start_WithOptionOnReschedule_OnRescheduleCalledWithCorrectArguments()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnReschedule = handler.OnReschedule };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => handler.OnReschedule(scheduledAction, TimeSpan.FromMilliseconds(20)))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => handler.OnReschedule(scheduledAction, TimeSpan.FromSeconds(1)))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Start_WithOptionOnReschedule_OnRescheduleCalledAfterError()
        {
            var action = A.Fake<IAction>();
            A.CallTo(() => action.DoSomething())
             .Throws<InvalidOperationException>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnReschedule = handler.OnReschedule };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => handler.OnReschedule(A<IScheduledAction>.Ignored, A<TimeSpan>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void Start_WithOptionAfterFailureSchedule_AlternativeScheduleIsUsedAfterError()
        {
            var action = A.Fake<IAction>();
            A.CallTo(() => action.DoSomething())
             .Throws<InvalidOperationException>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var failureSchedule = A.Fake<ISchedule>();
            A.CallTo(() => failureSchedule.NextEventAfter())
             .Returns(TimeSpan.FromHours(1));
            var handler = A.Fake<IHandler>();
            var options = new Options() { AfterFailureSchedule = failureSchedule, OnReschedule = handler.OnReschedule };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => failureSchedule.NextEventAfter())
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => handler.OnReschedule(scheduledAction, TimeSpan.FromHours(1)))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Start_WithOptionAfterFailureSchedule_MainScheduleIsUsedAfterErrorRecovery()
        {
            var action = A.Fake<IAction>();
            A.CallTo(() => action.DoSomething())
             .Throws<InvalidOperationException>()
             .NumberOfTimes(1)
             .Then
             .DoesNothing();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var failureSchedule = A.Fake<ISchedule>();
            A.CallTo(() => failureSchedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(30));
            var handler = A.Fake<IHandler>();
            var options = new Options() { AfterFailureSchedule = failureSchedule, OnReschedule = handler.OnReschedule };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => failureSchedule.NextEventAfter())
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => handler.OnReschedule(scheduledAction, TimeSpan.FromMilliseconds(20)))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => handler.OnReschedule(scheduledAction, TimeSpan.FromMilliseconds(30)))
             .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => handler.OnReschedule(scheduledAction, TimeSpan.FromSeconds(1)))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Start_WithOptionOnError_OnErrorCalled()
        {
            var action = A.Fake<IAction>();
            A.CallTo(() => action.DoSomething())
             .Throws<InvalidOperationException>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnError = handler.OnError };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => handler.OnError(A<IScheduledAction>.Ignored, A<Exception>.Ignored))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Start_WithOptionOnError_OnErrorCalledWithCorrectArguments()
        {
            var action = A.Fake<IAction>();
            A.CallTo(() => action.DoSomething())
             .Throws<InvalidOperationException>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.FromMilliseconds(20))
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromSeconds(1));
            var handler = A.Fake<IHandler>();
            var options = new Options() { OnError = handler.OnError };
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule, options);

            scheduledAction.Start();

            Thread.Sleep(100);
            A.CallTo(() => handler.OnError(scheduledAction, A<Exception>.That.IsInstanceOf(typeof(InvalidOperationException))))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Stop_CallMultipleTimes_NoExceptionIsThrown()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter()).Returns(TimeSpan.FromMilliseconds(10));
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule);
            scheduledAction.Start();

            scheduledAction.Stop();
            scheduledAction.Stop();

            Assert.True(true);
        }

        [Fact]
        public void Start_WithScheduleReturningZero_ActionCalledImmediately()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter())
             .Returns(TimeSpan.Zero)
             .NumberOfTimes(1)
             .Then
             .Returns(TimeSpan.FromMilliseconds(-1));
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule);

            scheduledAction.Start();

            Thread.Sleep(50);
            A.CallTo(() => action.DoSomething())
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Start_WithScheduleReturningNegative_ScheduledActionIsStopped()
        {
            var action = A.Fake<IAction>();
            var schedule = A.Fake<ISchedule>();
            A.CallTo(() => schedule.NextEventAfter()).Returns(TimeSpan.FromMinutes(-1));
            var scheduledAction = new ScheduledAction(action.DoSomething, schedule);

            scheduledAction.Start();

            Thread.Sleep(50);
            A.CallTo(() => action.DoSomething())
             .MustNotHaveHappened();
        }
    }
}
