public interface IBookRepository
{
    bool Add(Book book);
    Book? GetById(int id); // nullable
    Book[] GetAll();
    bool Update(Book book);
    bool Delete(int id);
}

