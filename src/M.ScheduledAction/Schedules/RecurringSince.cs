using System;
using static System.Math;

namespace M.ScheduledAction.Schedules
{
    /// <summary>
    /// Represents a schedule recurring at given time interval since point in time.
    /// </summary>
    public class RecurringSince : ISchedule
    {
        private readonly TimeSpan interval;
        private readonly DateTime sinceDateTime;
        private readonly IDateTime dateTime;

        /// <summary>
        /// Creates a new instance of RecurringSince class.
        /// </summary>
        /// <param name="sinceDateTime">Point in time when event occurrences start.</param>
        /// <param name="interval">A time interval for the event occurrences.</param>
        /// <param name="dateTime">DateTime provider.</param>
        public RecurringSince(DateTime sinceDateTime, TimeSpan interval, IDateTime dateTime = null)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }

            this.interval = interval;
            this.sinceDateTime = sinceDateTime;
            this.dateTime = dateTime ?? SystemDateTime.Get();
        }

        /// <summary>
        /// Calculates the time interval until next scheduled event.
        /// </summary>
        /// <returns>Returns a TimeSpan representing the time until next scheduled event.</returns>
        public TimeSpan NextEventAfter()
        {
            TimeSpan nextEventAfter;
            TimeSpan timeSinceStart = dateTime.Now() - sinceDateTime;
            if (timeSinceStart.TotalMilliseconds < 0)
            {
                nextEventAfter = sinceDateTime - dateTime.Now();
            }
            else
            {
                double elapsedIntervals = Floor(timeSinceStart.TotalMilliseconds / interval.TotalMilliseconds);
                DateTime nextEventDateTime = sinceDateTime.AddMilliseconds((elapsedIntervals + 1) * interval.TotalMilliseconds);
                nextEventAfter = nextEventDateTime - dateTime.Now();
            }

            return nextEventAfter;
        }
    }
}
