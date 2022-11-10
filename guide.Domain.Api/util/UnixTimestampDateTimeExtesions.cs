namespace CoreAPI.Utilities
{
  public static class UnixTimestampDateTimeExtesions
  {
    public static DateTime UnixTimestampToDateTime(this double  unixTime)
    {
      DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
      long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
      return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
    }
    public static double DateTimeToUnixTimestamp(this DateTime dateTime)
    {
      DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
      long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
      return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
    }
  }
}