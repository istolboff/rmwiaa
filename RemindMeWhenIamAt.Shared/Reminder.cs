namespace RemindMeWhenIamAt.SharedCode
{
    public sealed class Reminder
    {
        public GeoLocation Location { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
