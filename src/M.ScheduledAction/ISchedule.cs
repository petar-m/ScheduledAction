using System;

namespace M.ScheduledAction
{
    /// <summary>
    /// Represents a sequence of events by getting the time interval to the next scheduled event.
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// Calculates the time interval until next scheduled event.
        /// </summary>
        /// <returns>Returns a TimeSpan representing the time until next scheduled event. Negative TimeSpan denotes there are no more events.</returns>
        TimeSpan NextEventAfter();
    }
}
