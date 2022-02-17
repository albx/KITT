namespace KITT.Web.Models.Tools
{
    public record BotJobDetail
    {
        public string Name { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public bool IsRunning => Status.Equals("Running", StringComparison.InvariantCultureIgnoreCase);
    }
}
