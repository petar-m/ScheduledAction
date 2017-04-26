using System;

namespace M.ScheduledAction.Schedules
{
    /// <summary>
    /// A schedule representing fixed time interval.
    /// </summary>
    public class Fixed : ISchedule
    {
        private readonly TimeSpan interval;

        /// <summary>
        /// Create a new instance of Fixed class.
        /// </summary>
        /// <param name="interval">The time interval until next event occurence.</param>
        public Fixed(TimeSpan interval)
        {
            if(interval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }

            this.interval = interval;
        }

        /// <summary>
        /// Always returns the same interval.
        /// </summary>
        /// <returns>Returns a TimeSpan representing the time until next scheduled event.</returns>
        public TimeSpan NextEventAfter()
        {
            return interval;
        }
    }
}
