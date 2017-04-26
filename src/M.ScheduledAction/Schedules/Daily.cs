using System;

namespace M.ScheduledAction.Schedules
{
    /// <summary>
    /// A schedule having single event per day.
    /// </summary>
    public class Daily : ISchedule
    {
        private static readonly TimeSpan maxTimeSpan = new TimeSpan(0, 23, 59, 59, 999);

        private readonly TimeSpan timeOfDay;
        private readonly IDateTime dateTime;

        /// <summary>
        /// Creates a new instance of Daily class.
        /// </summary>
        /// <param name="timeOfDay">The time of the day when event occurs.</param>
        /// <param name="dateTime">DateTime provider.</param>
        public Daily(TimeSpan timeOfDay, IDateTime dateTime = null)
        {
            if (timeOfDay < TimeSpan.Zero || timeOfDay > maxTimeSpan)
            {
                throw new ArgumentOutOfRangeException(nameof(timeOfDay));
            }

            this.timeOfDay = timeOfDay;
            this.dateTime = dateTime ?? SystemDateTime.Get();
        }

        /// <summary>
        /// Calculates the time interval until next scheduled event.
        /// </summary>
        /// <returns>Returns a TimeSpan representing the time until next scheduled event.</returns>
        public TimeSpan NextEventAfter()
        {
            DateTime now = dateTime.Now();
            DateTime scheduleDate = now.Date.Add(timeOfDay);
            if (scheduleDate >= now)
            {
                return scheduleDate - now;
            }
            else
            {
                return scheduleDate.AddDays(1) - now;
            }
        }
    }
}
