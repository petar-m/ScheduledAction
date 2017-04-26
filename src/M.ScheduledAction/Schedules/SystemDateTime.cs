using System;

namespace M.ScheduledAction.Schedules
{
    /// <summary>
    /// Represents DateTime provided by the current computer.
    /// </summary>
    public class SystemDateTime : IDateTime
    {
        private static readonly IDateTime systemTime = new SystemDateTime();

        private SystemDateTime()
        {
        }

        /// <summary>
        /// Gets an instance of SystemDateTime.
        /// </summary>
        /// <returns>Returns an instance of SystemDateTime</returns>
        public static IDateTime Get()
        {
            return systemTime;
        }

        /// <summary>
        /// Gets a DateTime object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        /// <returns>An object whose value is the current local date and time.</returns>
        public DateTime Now() => DateTime.Now;
    }
}
