namespace Catalog.Books;

public record StoreBooksRequest(Book Book, Author Author, int Amount);