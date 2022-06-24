namespace MetricsManagement.Agent.Data
{
    public abstract class Repository
    {
        private static List<Metric> _metrics = new();
        public string TableName { get; set; }

        public void Create(int value, long time)
        {
            _metrics.Add(new(){Value = value, Time = time});
        }

        public IEnumerable<Metric> Get(DateTimeOffset from, DateTimeOffset to)
        {
            var fromSeconds = from.ToUnixTimeSeconds();
            var toSeconds = to.ToUnixTimeSeconds();

            if (fromSeconds == toSeconds)
            {
                return _metrics.Where(e => e.Time == fromSeconds);
            }

            var (min, max) = fromSeconds > toSeconds
                ? (fromSeconds, toSeconds)
                : (toSeconds, fromSeconds);

            return _metrics.Where(e=>e.Time >= min && e.Time < max);
        }
    }
}