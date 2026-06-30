// Library management class
public class Library
{
    private Book[] books;
    private int count;

    public Library(int capacity = 100)
    {
        books = new Book[capacity];
        count = 0;
    }

    public void AddBook(Book book)
    {
        if (count >= books.Length || FindById(book.Id) != null)
        {
            Console.WriteLine("\nError, book not added");
            return; // break 
        }
        books[count] = book;
        count++;
        Console.WriteLine("\nBook added successfully");
    }

    public Book FindById(int id)
    {
        for (int i = 0; i < count; i++)
        {
            if (books[i].Id == id)
            {
                return books[i];
            }
        }
        return null; // if book not found return null
    }

    public bool BorrowBook(int id)
    {
        Book book = FindById(id);
        if (book != null)
        {
            return book.Borrow();
        }
        return false;
    }

    public bool ReturnBook(int id)
    {
        Book book = FindById(id);
        if (book != null)
        {
            return book.Return();
        }
        return false;
    }

    public void PrintAllBooks()
    {
        for (int i = 0; i < count; i++)
        {
            books[i].PrintInfo();
        }
    }



    public void AudioBookReport()
    {
        int totalDuration = 0;
        int maxDuration = 0;

        for (int i = 0; i < count; i++)
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

    public void PhysicalBookReport()
    {
        bool first = true;
        for (int i = 0; i < count; i++)
        {
            PhysicalBook pb = books[i] as PhysicalBook;
            if (pb != null && pb.AvailableCopies == 0)
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

    public void DigitalBookReport()
    {
        double totalSize = 0;
        double maxSize = 0;

        for (int i = 0; i < count; i++)
        {
            DigitalBook db = books[i] as DigitalBook;
            if (db != null)
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
}