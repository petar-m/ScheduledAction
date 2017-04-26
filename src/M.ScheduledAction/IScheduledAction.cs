namespace M.ScheduledAction
{
    /// <summary>
    /// Represents an action executed by some schedule.
    /// </summary>
    public interface IScheduledAction
    {
        /// <summary>
        /// Starts the execution of action by some schedule.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the execution of action.
        /// </summary>
        void Stop();
    }
}