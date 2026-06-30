
// Audio book subclass
public class AudioBook : Book
{
    private int _durationMinutes;
    private string _narrator;

    public AudioBook(int id, string title, string author, int durationMinutes, string narrator)
        : base(id, title, author)
    {
        DurationMinutes = durationMinutes;
        Narrator = narrator;
    }

    public int DurationMinutes
    {
        get { return _durationMinutes; }
        private set
        {
            if (value >= 0)
            {
                _durationMinutes = value;
            }
            else
            {
                _durationMinutes = 0; // Default fallback to prevent negative lengths
            }
        }
    }

    public string Narrator
    {
        get { return _narrator; }
        private set
        {    //  check if string is null, empty, or just whitespace.
            if (string.IsNullOrWhiteSpace(value))
            {
                _narrator = "UNKNOWN NARRATOR";
            }
            else
            {
                _narrator = value;
            }
        }
    }
    public override void PrintInfo()
    {
        Console.Write("Audio Book; ");
        base.PrintInfo(); // Reuses base printing structure polymorphic style
        Console.Write($"; {DurationMinutes}; {Narrator}\n");
    }
}
