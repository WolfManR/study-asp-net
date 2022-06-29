namespace MetricsManagement.Manager.Data;

public interface IAgentsStorageStrategy
{
    void Disable(int id);
    void Enable(int id);
    IEnumerable<Agent> Get();
    int Register(string uri);
}