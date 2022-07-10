namespace Catalog.Books;

public class Author : Person
{
    public string FullName => FirstName + " " + LastName;
}