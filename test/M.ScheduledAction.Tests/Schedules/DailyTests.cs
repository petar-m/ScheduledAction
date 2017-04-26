using System;
using M.ScheduledAction.Schedules;
using Xunit;

namespace M.ScheduledAction.Tests.Schedules
{
    public class DailyTests
    {
        public class TestCase
        {
            public DateTime Current { get; set; }
            public TimeSpan TimeOfDay { get; set; }
            public DateTime Expected { get; set; }
        }

        public static object[][] TestCases =
        {
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23,  0,  0,  0,   0), TimeOfDay = TimeSpan.Zero, Expected = new DateTime(2017, 11, 23) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23,  0,  0,  0,   1), TimeOfDay = TimeSpan.Zero, Expected = new DateTime(2017, 11, 24) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 23, 59, 59, 999), TimeOfDay = TimeSpan.Zero, Expected = new DateTime(2017, 11, 24) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20,  0,  0,  0,   0), TimeOfDay = new TimeSpan(0, 23, 59, 59, 999), Expected = new DateTime(2017, 11, 20, 23, 59, 59, 999) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20,  0,  0,  0,   1), TimeOfDay = new TimeSpan(0, 23, 59, 59, 999), Expected = new DateTime(2017, 11, 20, 23, 59, 59, 999) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20, 12, 33, 45,  17), TimeOfDay = new TimeSpan(0, 23, 59, 59, 999), Expected = new DateTime(2017, 11, 20, 23, 59, 59, 999) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20, 23, 59, 59, 999), TimeOfDay = new TimeSpan(0, 23, 59, 59, 999), Expected = new DateTime(2017, 11, 20, 23, 59, 59, 999) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20,  1, 23,  3, 312), TimeOfDay = new TimeSpan(0, 12, 30,  0,   0), Expected = new DateTime(2017, 11, 20, 12, 30,  0,   0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20, 10, 54, 31,   1), TimeOfDay = new TimeSpan(0, 12, 30,  0,   0), Expected = new DateTime(2017, 11, 20, 12, 30,  0,   0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20, 16,  0, 45,  17), TimeOfDay = new TimeSpan(0, 12, 30,  0,   0), Expected = new DateTime(2017, 11, 21, 12, 30,  0,   0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 20, 21, 30,  9, 656), TimeOfDay = new TimeSpan(0, 12, 30,  0,   0), Expected = new DateTime(2017, 11, 21, 12, 30,  0,   0) } },
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void NextEventAfter_WithTimeOfDay_TimeUntilRun(TestCase testCase)
        {
            var dateTime = new TestDateTime(testCase.Current);
            var schedule = new Daily(testCase.TimeOfDay, dateTime);

            TimeSpan runAfter = schedule.NextEventAfter();

            Assert.Equal(testCase.Expected, dateTime.Now().Add(runAfter));
        }

        [Fact]
        public void New_WithNegativeTimeSpan_Throws()
        {
            var negativeTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Daily(negativeTimeSpan));
        }

        [Fact]
        public void New_WithTimeSpanGreaterThan24Hours_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Daily(new TimeSpan(24, 0, 0)));
        }

        [Fact]
        public void New_WithoutIDateTime_HasDefault()
        {
            var schedule = new Daily(new TimeSpan(10, 0, 0));

            TimeSpan timeToNextRun = schedule.NextEventAfter();

            Assert.True(true);
        }
    }
}
