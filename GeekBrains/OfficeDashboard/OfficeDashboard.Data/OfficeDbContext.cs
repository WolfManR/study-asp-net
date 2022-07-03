using Microsoft.EntityFrameworkCore;

namespace OfficeDashboard.Data;

public class OfficeDbContext : DbContext
{
    public OfficeDbContext(DbContextOptions<OfficeDbContext> options) : base(options) { }

    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
    public DbSet<OfficeEntity> Offices => Set<OfficeEntity>();
}