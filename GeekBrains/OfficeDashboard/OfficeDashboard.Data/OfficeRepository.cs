using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace OfficeDashboard.Data;

public class OfficeRepository
{
    private readonly OfficeDbContext _dbContext;
    // ReSharper disable once InconsistentNaming
    private readonly string NameOfNotAssignedEmployeesOffice;

    public OfficeRepository(OfficeDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        NameOfNotAssignedEmployeesOffice = configuration[nameof(NameOfNotAssignedEmployeesOffice)];
    }

    public Task<List<OfficeSelectListItem>> GetOffices()
    {
        return _dbContext.Offices
            .AsNoTracking()
            .Select(o => new OfficeSelectListItem()
            {
                Id = o.Id,
                Name = o.Name
            }).ToListAsync();
    }

    public async Task<Office> GetOffice(Guid officeId)
    {
        if (await _dbContext.Offices.AsNoTracking().Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == officeId) is not { } office) return null;
        return new Office()
        {
            Id = officeId,
            Name = office.Name,
            Employees = office.Employees.Select(e => new ListEmployee()
            {
                Id = e.Id,
                Name = e.FirstName,
                Surname = e.LastName
            }).ToList()
        };
    }

    public async Task<EditOffice> GetOfficeForEdit(Guid officeId)
    {
        if (await _dbContext.Offices.AsNoTracking().FirstOrDefaultAsync(o => o.Id == officeId) is not { } office) return null;
        return new EditOffice()
        {
            Id = officeId,
            Name = office.Name
        };
    }

    public async Task<IReadOnlyCollection<ListEmployee>> GetEmployees(Guid officeId)
    {
        if (await _dbContext.Offices.AsNoTracking().Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == officeId) is not { } office) return Array.Empty<ListEmployee>();
        return office.Employees.Select(e => new ListEmployee()
        {
            Id = e.Id,
            Name = e.FirstName,
            Surname = e.LastName
        }).ToList();
    }

    public async Task<EditEmployee> GetEmployeeForEdit(Guid id)
    {
        if (await _dbContext.Employees.AsNoTracking().Include(e => e.Office).FirstOrDefaultAsync(e => e.Id == id) is not { } employee) return null;
        return new EditEmployee()
        {
            Id = employee.Id,
            Name = employee.FirstName,
            Surname = employee.LastName,
            OfficeId = employee.Office?.Id ?? Guid.Empty
        };
    }

    public async Task<Guid> RegisterOffice(CreateOffice office)
    {
        var entity = new OfficeEntity()
        {
            Name = office.Name
        };

        _dbContext.Offices.Add(entity);
        await _dbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<Guid> RegisterEmployee(CreateEmployee employee)
    {
        var entity = new EmployeeEntity()
        {
            FirstName = employee.Name,
            LastName = employee.Surname
        };

        if (await _dbContext.Offices.Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == employee.OfficeId) is { } office)
        {
            entity.Office = office;
        }

        await _dbContext.Employees.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> RemoveEmployee(Guid employeeId)
    {
        if (await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId) is not { } employee) return false;

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveOffice(Guid officeId)
    {
        if (await _dbContext.Offices.AsNoTracking().Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == officeId) is not { } office) return false;

        _dbContext.Offices.Remove(office);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateEmployeeData(EditEmployee editEmployee)
    {
        if (await _dbContext.Employees.Include(e => e.Office).FirstOrDefaultAsync(e => e.Id == editEmployee.Id) is not { } employee) return false;


        employee.FirstName = editEmployee.Name;
        employee.LastName = editEmployee.Surname;

        var newOfficeId = editEmployee.OfficeId;
        if (newOfficeId != Guid.Empty && newOfficeId != employee.Office?.Id && await _dbContext.Offices.FirstOrDefaultAsync(o => o.Id == newOfficeId) is { } newOffice)
        {
            employee.Office = newOffice;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateOfficeData(EditOffice updated)
    {
        if (await _dbContext.Offices.AsNoTracking().Include(o => o.Employees).FirstOrDefaultAsync(o => o.Id == updated.Id) is not { } office) return false;

        office.Name = updated.Name;
        return true;
    }

    public async Task<IReadOnlyCollection<ListEmployee>> GetEmployeesThatNotAssignedToAnyOffice()
    {
        var employees = await _dbContext.Employees
            .AsNoTracking()
            .Include(e => e.Office)
            .Where(e => e.Office == null)
            .Select(e => new ListEmployee
            {
                Id = e.Id,
                Name = e.FirstName,
                Surname = e.LastName
            })
            .ToListAsync();
        return employees;
    }

    public async Task<bool> IsContainsNotAssignedToOfficeEmployees() =>
        await _dbContext.Employees.AsNoTracking().Include(e => e.Office).AnyAsync(employee => employee.Office == null);
}