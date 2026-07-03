// Physical book subclass
public class PhysicalBook : Book
{
    private int _pages;
    private int _availableCopies;

    public PhysicalBook(int id, string title, string author, int pages, int availableCopies)
        : base(id, title, author)
    {
        Pages = pages;
        AvailableCopies = availableCopies;
    }

    // Getter & Setters
    public int Pages
    {
        get { return _pages; }
        private set
        {
            if (value >= 0) // Enforce positive page numbers
            {
                _pages = value;
            }
        }
    }

    public int AvailableCopies
    {
        get { return _availableCopies; }
        private set
        {
            if (value >= 0) // Disallow negative inventory
            {
                _availableCopies = value;
            }
        }
    }

    public override bool Borrow() 
    {
        if (AvailableCopies > 0) // Cannot take more than total Available Copies
        {
            AvailableCopies--;
            return true;
        }
        return false;
    }

    public override bool Return()
    {
        AvailableCopies++;
        return true;        // No logic - therefore no bad case to return false.
    }

    public override void PrintInfo()
    {
        Console.Write("Physical Book; ");
        base.PrintInfo();
        Console.Write($"; {Pages}; {AvailableCopies}\n");
    }
}