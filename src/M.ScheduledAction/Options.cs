using System;
using System.Threading.Tasks;

namespace M.ScheduledAction
{
    /// <summary>
    /// Options for modifying the ScheduledAction behavior.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Indicates whether to execute the action when IScheduledAction.Start() is called. Default is false.
        /// </summary>
        public bool ExecuteOnStart { get; set; }
        
        /// <summary>
        /// Alternative schedule to use if exception is raised when executing by main schedule.
        /// After first success on alternative schedule, task is scheduled by main schedule.
        /// </summary>
        public ISchedule AfterFailureSchedule { get; set; }

        /// <summary>
        /// A delegate representing a method to be executed if exception is thrown during execution of the action.
        /// </summary>
        public Action<IScheduledAction, Exception> OnError { get; set; }

        /// <summary>
        /// A delegate representing a method to be executed when action is rescheduled.
        /// </summary>
        public Action<IScheduledAction, TimeSpan> OnReschedule { get; set; }

        /// <summary>
        /// Invokes the OnError callback on ThreadPool thread.
        /// </summary>
        /// <param name="scheduledAction">The instance of ScheduledAction where the exception was caught.</param>
        /// <param name="error">The actual exception</param>
        public void InvokeOnError(IScheduledAction scheduledAction, Exception error)
        {
            if (OnError != null)
            {
                Task.Run(() => OnError(scheduledAction, error));
            }
        }

        /// <summary>
        /// Invokes the OnReschedule callback on ThreadPool thread.
        /// </summary>
        /// <param name="scheduledAction">The instance of ScheduledAction where the reschedule occurred.</param>
        /// <param name="nextEventAt">Time interval to next event.</param>
        public void InvokeOnReschedule(IScheduledAction scheduledAction, TimeSpan nextEventAt)
        {
            if (OnReschedule != null)
            {
                Task.Run(() => OnReschedule(scheduledAction, nextEventAt));
            }
        }
    }
}
