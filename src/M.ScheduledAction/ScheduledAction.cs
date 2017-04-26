using System;
using System.Threading;

namespace M.ScheduledAction
{
    /// <summary>
    /// Represents an action executed by given schedule.
    /// </summary>
    public class ScheduledAction : IDisposable, IScheduledAction
    {
        private static readonly Options Default = new Options();

        private readonly Action action;
        private readonly ISchedule schedule;
        private readonly Options options;

        private Timer timer;

        /// <summary>
        /// Creates a new instance of ScheduledAction.
        /// </summary>
        /// <param name="action">A delegate representing a method to be executed.</param>
        /// <param name="schedule">The schedule to be used.</param>
        /// <param name="options">Options for modifying the ScheduledAction behavior.</param>
        public ScheduledAction(Action action, ISchedule schedule, Options options = null)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.schedule = schedule ?? throw new ArgumentNullException(nameof(schedule)); 
            this.options = options ?? Default;
        }

        /// <summary>
        /// Starts the execution of action by given schedule.
        /// </summary>
        public void Start()
        {
            if (timer != null)
            {
                throw new InvalidOperationException("ScheduledAction is already started");
            }

            timer = new Timer(Execute, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if (options.ExecuteOnStart)
            {
                Reschedule(TimeSpan.Zero);
            }
            else
            {
                Reschedule(schedule);
            }
        }

        /// <summary>
        /// Stops the execution of action and releases all resources used by current instance of ScheduledAction.
        /// </summary>
        public void Stop()
        {
            if (timer == null)
            {
                return;
            }

            timer.Dispose();
            timer = null;
        }

        /// <summary>
        /// Releases all resources used by current instance of ScheduledAction.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        private void Execute(object sender)
        {
            if (timer == null)
            {
                return;
            }

            try
            {
                action();
                Reschedule(schedule);
            }
            catch (Exception exception)
            {
                options.InvokeOnError(this, exception);
                Reschedule(options.AfterFailureSchedule ?? schedule);
            }
        }

        private void Reschedule(ISchedule activeSchedule)
        {
            TimeSpan nextRunAt = activeSchedule.NextEventAfter();
            Reschedule(nextRunAt);
        }

        private void Reschedule(TimeSpan executeAfter)
        {
            if(executeAfter < TimeSpan.Zero)
            {
                Stop();
                return;
            }

            timer.Change(executeAfter, TimeSpan.FromMilliseconds(-1));

            DateTime nextRunAt = DateTime.Now.Add(executeAfter);
            options.InvokeOnReschedule(this, executeAfter);
        }
    }
}
