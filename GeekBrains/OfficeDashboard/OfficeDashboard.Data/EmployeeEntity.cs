using System.Collections.Generic;

namespace OfficeDashboard.Data;

public class EmployeeEntity
{
    public Guid Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public OfficeEntity Office { get; set; }
}