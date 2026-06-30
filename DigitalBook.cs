// Digital book subclass
public class DigitalBook : Book
{
    private double _fileSizeMB;
    private string _format;

    public DigitalBook(int id, string title, string author, double fileSizeMB, string format)
        : base(id, title, author)
    {
        FileSizeMB = fileSizeMB;
        Format = format;
    }

    public double FileSizeMB
    {
        get { return _fileSizeMB; }
        private set
        {
            if (value >= 0.0) // Prevent negative sizes
            {
                _fileSizeMB = value;
            }
            else
            {
                _fileSizeMB = 0.0;
            }
        }
    }

    public string Format
    {
        get { return _format; }
        private set
        {   // Guard clause against empty formats, defaults to PDF
            if (string.IsNullOrWhiteSpace(value))
            {
                _format = "no format";
            }
            else
            {
                _format = value;
            }
        }
    }

    public override void PrintInfo()
    {
        Console.Write("Digital Book; ");
        base.PrintInfo(); // Reuses base printing format
        Console.Write($"; {FileSizeMB}; {Format}\n");
    }
}
