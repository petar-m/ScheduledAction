using System;
using M.ScheduledAction.Schedules;
using Xunit;

namespace M.ScheduledAction.Tests.Schedules
{
    public class FixedTests
    {
        [Fact]
        public void New_WithValidInterval_Creates()
        {
            var interval = new TimeSpan(0, 1, 0);

            var schedule = new Fixed(interval);

            Assert.True(true);
        }

        [Fact]
        public void New_WithNegativeInterval_Throws()
        {
            var negativeTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Fixed(negativeTimeSpan));
        }

        [Fact]
        public void NextRun_WithPositiveInterval_ReturnsTheInterval()
        {
            var interval = new TimeSpan(1, 0, 0);
            var schedule = new Fixed(interval);

            TimeSpan nextRun = schedule.NextEventAfter();

            Assert.Equal(interval, nextRun);
        }
    }
}
