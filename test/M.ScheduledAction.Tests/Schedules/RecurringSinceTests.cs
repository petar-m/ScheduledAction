using System;
using M.ScheduledAction.Schedules;
using Xunit;

namespace M.ScheduledAction.Tests.Schedules
{
    public class RecurringSinceTests
    {
        public class TestCase
        {
            public DateTime Current { get; set; }
            public DateTime Since { get; set; }
            public TimeSpan Interval { get; set; }
            public DateTime Expected { get; set; }
        }

        public static object[][] TestCases =
        {
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23,  0,  0,  0,   0), Since = DateTime.MinValue, Interval = new TimeSpan(0, 0, 1), Expected = new DateTime(2017, 11, 23, 0, 0, 1, 0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23,  0,  0,  0,   0), Since = DateTime.MinValue, Interval = new TimeSpan(0, 0, 0, 0, 17), Expected = new DateTime(2017, 11, 23, 0, 0, 0, 11) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23,  0,  0,  0,   1), Since = new DateTime(2017, 12, 30, 23, 59, 59, 999), Interval = TimeSpan.FromHours(1), Expected = new DateTime(2017, 12, 30, 23, 59, 59, 999) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 10, 46, 55, 222), Since = new DateTime(2010,  1,  1,  0,  0,  0,   0), Interval = TimeSpan.FromHours(1), Expected = new DateTime(2017, 11, 23, 11,  0,  0,   0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 10, 46, 55, 222), Since = new DateTime(2010,  1,  1,  0,  0,  0,   0), Interval = TimeSpan.FromMinutes(1), Expected = new DateTime(2017, 11, 23, 10, 47,  0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 10, 46, 55, 222), Since = new DateTime(2010,  1,  1,  0,  0,  0,   0), Interval = TimeSpan.FromSeconds(1), Expected = new DateTime(2017, 11, 23, 10, 46, 56) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 10, 46, 55, 222), Since = new DateTime(2010,  1,  1,  0,  0,  0,   0), Interval = TimeSpan.FromMilliseconds(100), Expected = new DateTime(2017, 11, 23, 10, 46, 55, 300) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 10, 46, 55, 222), Since = new DateTime(2010,  1,  1,  0,  0,  0,   0), Interval = new TimeSpan(10, 30, 30), Expected = new DateTime(2017, 11, 23, 15, 53, 0, 0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 11, 23, 10, 46, 55, 222), Since = new DateTime(2010,  1,  1, 10, 30, 30,   0), Interval = new TimeSpan(10, 30, 30), Expected = new DateTime(2017, 11, 23, 15, 53, 0, 0) } },
            new object[] { new TestCase{ Current = new DateTime(2017, 04, 26, 13, 07, 55, 222), Since = new DateTime(2016,  1,  1,  8, 56,  0),      Interval = TimeSpan.FromHours(12),   Expected = new DateTime(2017,  4, 26, 20, 56, 0) } }
        };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void NextEventAfter_SinceDateWithPositiveInterval_TimeToNextRunIsCalculated(TestCase testCase)
        {
            var dateTime = new TestDateTime(testCase.Current);
            var schedule = new RecurringSince(testCase.Since, testCase.Interval, dateTime);

            TimeSpan nextEventAfter = schedule.NextEventAfter();

            Assert.Equal(testCase.Expected, testCase.Current.Add(nextEventAfter));
        }

        [Fact]
        public void New_WithNegativeTimeSpan_Throws()
        {
            var negativeTimeSpan = new TimeSpan(0, 0, 0, 0, -1);

            Assert.Throws<ArgumentOutOfRangeException>(() => new RecurringSince(DateTime.MinValue, negativeTimeSpan));
        }

        [Fact]
        public void New_WithTimeSpanZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RecurringSince(DateTime.MinValue, TimeSpan.Zero));
        }

        [Fact]
        public void New_WithoutIDateTime_HasDefault()
        {
            var schedule = new RecurringSince(DateTime.MinValue, new TimeSpan(0, 0, 0, 0, 1));

            TimeSpan nextEventAfter = schedule.NextEventAfter();

            Assert.True(true);
        }
    }
}
