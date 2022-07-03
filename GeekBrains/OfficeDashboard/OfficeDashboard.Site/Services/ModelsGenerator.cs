using Bogus;
using OfficeDashboard.Data;

namespace OfficeDashboard.Site.Services;

public class ModelsGenerator
{
    private static readonly Faker<Office> Offices = new Faker<Office>().Rules((faker, office) =>
    {
        office.Id = Guid.NewGuid();
        office.Name = faker.Company.CompanyName();
    });

    private static readonly Faker<ListEmployee> Employees = new Faker<ListEmployee>().Rules((faker, employ) =>
    {
        employ.Id = Guid.NewGuid();
        employ.Name = faker.Person.FirstName;
        employ.Surname = faker.Person.LastName;
    });

    public IEnumerable<ListEmployee> GenerateEmployees(int count = 1) => Employees.Generate(count);
    public Office GenerateOffice(int countEmployees = 1)
    {
        var office = Offices.Generate();
        foreach (var employ in GenerateEmployees(countEmployees))
        {
            office.Employees.Add(employ);
        }
        return office;
    }
}