namespace MetricsManagement.Manager.Data;

public class AgentsMemoryStorageStrategy : IAgentsStorageStrategy
{
    private static readonly List<Agent> Agents = new();

    public int Register(string uri)
    {
        var entity = new Agent { Uri = uri, Id = Agents.Count, IsEnabled = true };
        Agents.Add(entity);
        return entity.Id;
    }

    public void Enable(int id)
    {
        var agent = Agents.SingleOrDefault(e => e.Id == id);
        if (agent is null) return;
        agent.IsEnabled = true;
    }

    public void Disable(int id)
    {
        var agent = Agents.SingleOrDefault(e => e.Id == id);
        if (agent is null) return;
        agent.IsEnabled = false;
    }

    public IEnumerable<Agent> Get()
    {
        return Agents.ToArray();
    }
}