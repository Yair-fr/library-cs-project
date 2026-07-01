// Library management class
public class Library
{
    private IBookRepository repository;

    public Library(IBookRepository repository)
    {
        this.repository = repository;
    }

    public bool AddBook(Book book)
    {
        return repository.Add(book);
    }

    public bool BorrowBook(int id)
    {
        Book? book = repository.GetById(id);

        if (book == null)
        {
            return false;
        }

        if (!book.Borrow())
        {
            return false;
        }

        return repository.Update(book);
    }

    public bool ReturnBook(int id)
    {
        Book? book = repository.GetById(id);

        if (book == null)
        {
            return false;
        }

        if (!book.Return())
        {
            return false;
        }

        return repository.Update(book);
    }

    public void PrintAllBooks()
    {
        Book[] books = repository.GetAll();

        for (int i = 0; i < books.Length; i++)
        {
            books[i].PrintInfo();
        }
    }

    public void AudioBookReport()
    {
        Book[] books = repository.GetAll();

        int totalDuration = 0;
        int maxDuration = 0;

        for (int i = 0; i < books.Length; i++)
        {
            if (books[i] is AudioBook ab)
            {
                totalDuration += ab.DurationMinutes;

                if (ab.DurationMinutes > maxDuration)
                {
                    maxDuration = ab.DurationMinutes;
                }
            }
        }

        Console.WriteLine($"{totalDuration}; {maxDuration}");
    }

    public void DigitalBookReport()
    {
        Book[] books = repository.GetAll();

        double totalSize = 0;
        double maxSize = 0;

        for (int i = 0; i < books.Length; i++)
        {
            if (books[i] is DigitalBook db)
            {
                totalSize += db.FileSizeMB;

                if (db.FileSizeMB > maxSize)
                {
                    maxSize = db.FileSizeMB;
                }
            }
        }

        Console.WriteLine($"{totalSize}; {maxSize}");
    }

    public void PhysicalBookReport()
    {
        Book[] books = repository.GetAll();

        bool first = true;

        for (int i = 0; i < books.Length; i++)
        {
            if (books[i] is PhysicalBook pb && pb.AvailableCopies == 0)
            {
                if (!first)
                {
                    Console.Write("; ");
                }

                Console.Write($"{pb.Id}; {pb.Title}");
                first = false;
            }
        }

        if (!first)
        {
            Console.WriteLine();
        }
    }
}
