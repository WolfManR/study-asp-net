namespace OfficeDashboard.Data;

public class Office
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<ListEmployee> Employees { get; set; } = new HashSet<ListEmployee>();
}