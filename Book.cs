// Base class for books
public class Book
{
    // Private backing fields
    private int _id;
    private string _title;
    private string _author;

    // Constractor
    public Book(int id, string title, string author)
    {
        Id = id;            // Invokes Id setter validation
        Title = title;      // Invokes Title setter validation
        Author = author;    // Invokes Author setter validation
    }

    // Explicit property definitions - Getters & Setters
    public int Id
    {
        get { return _id; }
        protected set { _id = value; }  
    }

    public string Title
    {
        get { return _title; }
        protected set
        {
            //  check if string is null, empty, or just whitespace. 
            if (string.IsNullOrWhiteSpace(value))
            {
                _title = "No title";
            }
            else
            {
                _title = value;
            }
        }
    }

    public string Author
    {
        get { return _author; }
        protected set
        {
            //  check if string is null, empty, or just whitespace.
            if (string.IsNullOrWhiteSpace(value))
            {
                _author = "UN-KNOWN";
            }
            else
            {
                _author = value;
            }
        }
    }

    public virtual bool Borrow()
    {
        return true;
    }

    public virtual bool Return()
    {
        return true;
    }

    public virtual void PrintInfo()
    {
        Console.Write($"{Id}; {Title}; {Author}");
    }
}