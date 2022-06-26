namespace MetricsManagement.Manager.Data
{
    public class Agent
    {
        public Agent(string uri) => Uri = uri;

        public int Id { get; init; }
        public string Uri { get; }
        public bool IsEnabled { get; set; }
    }
}