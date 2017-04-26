using System;
using M.ScheduledAction.Schedules;
using Xunit;

namespace M.ScheduledAction.Tests.Schedules
{
    public class SystemDateTimeTests
    {
        [Fact]
        public void Now_ReturnsLocalTime()
        {
            DateTime now = SystemDateTime.Get().Now();

            Assert.Equal(DateTimeKind.Local, now.Kind);
        }

        [Fact]
        public void Now_ReturnsCurrentTime()
        {
            var expected = DateTime.Now;

            var now = SystemDateTime.Get().Now();

            Assert.True(now - expected < new TimeSpan(0, 0, 0, 0, 100));
        }
    }
}
