// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.Common
{
    public class UnixTimeTest
    {
        [Fact]
        public void FromDateTimeUtc()
        {
            UnixTime timestamp = new DateTime(2004, 09, 16, 0, 0, 0, DateTimeKind.Utc);
            timestamp.Should().Be(12677 /*days*/ * 86400 /*seconds*/);
        }

        [Fact]
        public void FromDateTimeLocal()
        {
            UnixTime timestamp = new DateTime(2004, 09, 16, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            timestamp.Should().Be(12677 /*days*/ * 86400 /*seconds*/);
        }

        [Fact]
        public void FromDateTimeUnspecified()
        {
            UnixTime timestamp = new DateTime(2004, 09, 16, 0, 0, 0, DateTimeKind.Unspecified);
            timestamp.Should().Be(12677 /*days*/ * 86400 /*seconds*/);
        }

        [Fact]
        public void FromDateTimeOffset()
        {
            UnixTime timestamp = new DateTimeOffset(2004, 09, 16, 2, 0, 0, TimeSpan.FromHours(2));
            timestamp.Should().Be(12677 /*days*/ * 86400 /*seconds*/);
        }

        [Fact]
        public void ToDateTime()
        {
            DateTime dateTime = new UnixTime(12677 /*days*/ * 86400 /*seconds*/);
            dateTime.Should().Be(new DateTime(2004, 09, 16, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void ToDateTimeOffset()
        {
            DateTimeOffset dateTime = new UnixTime(12677 /*days*/ * 86400 /*seconds*/);
            dateTime.Should().Be(new DateTimeOffset(2004, 09, 16, 0, 0, 0, TimeSpan.Zero));
        }
    }
}
