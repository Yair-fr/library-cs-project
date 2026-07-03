using System;


/*
 *  Project Authors: Yair Friedensohn & Omri Vigodman & Arad Tal
 */


public class Program
{

    // new flag for error
    const string InvalidInputMessage = "Invalid input. Please try again.";
    const string DatabaseErrorMessage = "Database error occurred. Operation failed.";
    public static void Main(string[] args)
    {
        IBookRepository repository = new MySqlBookRepository();
        bool running = true;

        string txt_menu = $"\n1. Add physical book \n" +
                            "2. Add digital book \n" +
                            "3. Add audio book \n" +
                            "4. List all books \n" +
                            "5. Borrow book \n" +
                            "6. Return book \n" +
                            "7. Digital book report \n" +
                            "8. Audio book report \n" +
                            "9. Physical book report \n" +
                            "10. Exit \n\n";

        while (running)
        {
            string choice = GetInput(txt_menu);

            switch (choice)
            {
                case "1":
                case "2":
                case "3":
                    ExecuteAddBook(repository, choice);
                    break;
                case "4":
                    PrintAllBooksUI(repository);
                    break;
                case "5":
                    BorrowBookUI(repository);

                    break;
                case "6":
                    ReturnBookUI(repository);
                    break;
                case "7":
                    DigitalBookReport(repository);
                    break;
                case "8":
                    AudioBookReport(repository);
                    break;
                case "9":
                    PhysicalBookReport(repository); 
                    break;
                case "10":
                    running = false;
                    break;
            }
        }
    }



    // Helper validation inputs
    static bool TryReadNonNegativeInt(string input, out int value)
    {
        return int.TryParse(input, out value) && value >= 0;
    }
    // Replace for GetDouble 
    static bool TryReadNonNegativeDouble(string input, out double value)
    {
        return double.TryParse(input, out value) && value >= 0;
    }
    // DRY helper methods for console input
    static string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? ""; // if null than empty string ""
    }
















    // Unified addition UI mapping directly into repo.Add()
    static void ExecuteAddBook(IBookRepository repository, string user_choice)
    {

        string idInput = GetInput("Enter ID: ");
        string title = GetInput("Enter Title: ");
        string author = GetInput("Enter Author: ");

        switch (user_choice)
        {
            case "1":
                {
                    string pagesInput = GetInput("Enter Pages: ");
                    string copiesInput = GetInput("Enter Available Copies: ");

                    bool idOk = TryReadNonNegativeInt(idInput, out int id);
                    bool pagesOk = TryReadNonNegativeInt(pagesInput, out int pages);
                    bool copiesOk = TryReadNonNegativeInt(copiesInput, out int copies);

                    if (!idOk || !pagesOk || !copiesOk)
                    {
                        Console.WriteLine(InvalidInputMessage);
                        return;
                    }

                    PhysicalBook pb = new PhysicalBook(id, title, author, pages, copies);
                    if (repository.Add(pb))
                    {
                        Console.WriteLine("\nBook added successfully");
                    }
                    else
                    {
                        Console.WriteLine(DatabaseErrorMessage);
                    }
                    break;
                }

            case "2":
                {
                    string sizeInput = GetInput("Enter File Size: ");
                    string format = GetInput("Enter Format: ");

                    bool idOk = TryReadNonNegativeInt(idInput, out int id);
                    bool sizeOk = TryReadNonNegativeDouble(sizeInput, out double size);

                    if (!idOk || !sizeOk)
                    {
                        Console.WriteLine(InvalidInputMessage);
                        return;
                    }

                    DigitalBook db = new DigitalBook(id, title, author, size, format);
                    if (repository.Add(db))
                    {
                        Console.WriteLine("\nBook added successfully");
                    }
                    else
                    {
                        Console.WriteLine(DatabaseErrorMessage);
                    }
                    break;
                }

            case "3":
                {
                    string durationInput = GetInput("Enter Duration: ");
                    string narrator = GetInput("Enter Narrator: ");

                    bool idOk = TryReadNonNegativeInt(idInput, out int id);
                    bool durationOk = TryReadNonNegativeInt(durationInput, out int duration);

                    if (!idOk || !durationOk)
                    {
                        Console.WriteLine(InvalidInputMessage);
                        return;
                    }

                    AudioBook ab = new AudioBook(id, title, author, duration, narrator);
                    if (repository.Add(ab))
                    {
                        Console.WriteLine("\nBook added successfully");
                    }
                    else
                    {
                        Console.WriteLine(DatabaseErrorMessage);
                    }
                    break;
                }
        }
    }




    // Prints all items coming out of your modern List/Array GetAll database query
    static void PrintAllBooksUI(IBookRepository repository)
    {
        Book[] bk = repository.GetAll();
        foreach (Book item in bk)
        {
            if (item == null)
            {
                break;
            }
            else
            {
                item.PrintInfo();;
            }
        }
    }



    // Borrow logic using repository actions
    static void BorrowBookUI(IBookRepository repository)
    {
        string idInput = GetInput("Enter Book ID to Borrow: ");
        if (!TryReadNonNegativeInt(idInput, out int id))
        {
            Console.WriteLine(InvalidInputMessage);
            return;
        }

        Book? book = repository.GetById(id);
        if (book == null)
        {
            Console.WriteLine(DatabaseErrorMessage); // NEED TO SEE IF MSG IS CORRECT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return;
        }

        switch (book)
        {
            case PhysicalBook p:
                if (p.Borrow()) // update total -= 1
                {
                    if (repository.Update(p))
                    {
                        Console.WriteLine("Book borrowed successfully!");
                    }
                    else
                    {
                        Console.WriteLine(DatabaseErrorMessage);
                    }
                }
                break;

            case DigitalBook d:  // should always work if found on the library_db
            case AudioBook a:    // should always work if found on the library_db
                Console.WriteLine("Book borrowed successfully!");
                break;

            default:    // null case -safty should not occur ->  if (book == null) than print error
                break;
        }
    }




    // Return logic using repository actions
    static void ReturnBookUI(IBookRepository repository)
    {
        string idInput = GetInput("Enter Book ID to Return: ");
        if (!TryReadNonNegativeInt(idInput, out int id))
        {
            Console.WriteLine(InvalidInputMessage);
            return;
        }

        Book? book = repository.GetById(id);
        if (book == null)
        {
            Console.WriteLine(DatabaseErrorMessage);
            return;
        }

        switch (book)
        {
            case PhysicalBook p:
                if (p.Return()) // update total += 1
                {
                    if (repository.Update(p))
                    {
                        Console.WriteLine("Book borrowed successfully!");
                    }
                    else
                    {
                        Console.WriteLine(DatabaseErrorMessage);
                    }
                }
                break;

            case DigitalBook d:  // should always work if found on the library_db
            case AudioBook a:    // should always work if found on the library_db
                Console.WriteLine("Book returned successfully!");
                break;

            default:    // null case -safty should not occur ->  if (book == null) than print error
                break;
        }
    }


    static void AudioBookReport(IBookRepository repository)
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

    static void DigitalBookReport(IBookRepository repository)
    {
        Book[] books = repository.GetAll();
        double totalSize = 0;
        double maxSize = 0;

        for (int i = 0; i < books.Length; i++)
        {
            if (books[i] is DigitalBook db)
            {
                // Note: Make sure FileSizeMB matches your property casing exactly (FileSizeMB vs FileSizeMb)
                totalSize += db.FileSizeMB;

                if (db.FileSizeMB > maxSize)
                {
                    maxSize = db.FileSizeMB;
                }
            }
        }

        Console.WriteLine($"{totalSize}; {maxSize}");
    }

    static void PhysicalBookReport(IBookRepository repository)
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