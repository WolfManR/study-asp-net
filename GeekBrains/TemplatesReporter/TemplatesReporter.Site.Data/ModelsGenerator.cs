using Bogus;

namespace TemplatesReporter.Site.Data;

public class ModelsGenerator
{
    private Faker<Book> bookFaker = new Faker<Book>().Rules((f, b) =>
    {
        b.Author = f.Person.FullName;
        b.Title = f.Commerce.ProductName();
        b.Pages = f.Random.Int(1, 1480);
    });

    public IEnumerable<Book> GenerateBooks(int count = 1) => bookFaker.Generate(count);
}