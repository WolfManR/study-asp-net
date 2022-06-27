namespace MetricsManagement.Manager.Data;

public class AgentsRepository
{
    private static readonly List<Agent> _agents = new List<Agent>();

    public int Register(string uri)
    {
        var entity = new Agent(uri) {Id = _agents.Count, IsEnabled = true };
        _agents.Add(entity);
        return entity.Id;
    }

    public void Enable(int id)
    {
        var agent = _agents.SingleOrDefault(e => e.Id == id);
        if(agent is null) return;
        agent.IsEnabled = true;
    }

    public void Disable(int id)
    {
        var agent = _agents.SingleOrDefault(e => e.Id == id);
        if(agent is null) return;
        agent.IsEnabled = false;
    }

    public IEnumerable<Agent> Get()
    {
        return _agents.ToArray();
    }
}