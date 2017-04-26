using System;
using M.ScheduledAction.Schedules;

namespace M.ScheduledAction.Tests.Schedules
{
    internal class TestDateTime : IDateTime
    {
        private readonly DateTime now;

        public TestDateTime(DateTime now)
        {
            this.now = now;
        }

        public DateTime Now() => now;
    }
}
