namespace MetricsManagement.Manager.Data;

public class AgentsRepository
{
    private readonly IAgentsStorageStrategy _storageStrategy;

    public AgentsRepository(IAgentsStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    public int Register(string uri)
    {
        return _storageStrategy.Register(uri);
    }

    public void Enable(int id)
    {
        _storageStrategy.Enable(id);
    }

    public void Disable(int id)
    {
        _storageStrategy.Disable(id);
    }

    public IEnumerable<Agent> Get()
    {
        return _storageStrategy.Get();
    }
}