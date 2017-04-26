using System;

namespace M.ScheduledAction.Schedules
{
    /// <summary>
    /// Provides time information.
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gets the current DateTime.
        /// </summary>
        /// <returns>Retrns the current DateTime.</returns>
        DateTime Now();
    }
}
