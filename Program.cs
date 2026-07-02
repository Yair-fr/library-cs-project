using System;


/*
 *  Project Authors: Yair Friedensohn & Omri Vigodman & Arad Tal
 */



// Application entry point
public class Program
{

    // new flag for error
    const string InvalidInputMessage = "Invalid input. Please try again.";
    const string DatabaseErrorMessage = "Database error occurred. Operation failed.";
    public static void Main(string[] args)
    {
        IBookRepository repository = new MySqlBookRepository();
        // Library library = new Library(repository);
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
                    // Pass 'repository' instead of 'library'
                    ExecuteAddBook(repository, choice);
                    break;
                case "4":
                    // Pull all books directly from the database and print them
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
                    // ExecuteReportUI(repository, "DIGITAL");
                    break;
                case "8":
                    AudioBookReport(repository);
                    // ExecuteReportUI(repository, "AUDIO");
                    break;
                case "9":
                    PhysicalBookReport(repository); // Changed from library.PhysicalBookReport();
                    // ExecuteReportUI(repository, "PHYSICAL");
                    break;
                // case "1": // CHANGE REQUIRED: Pass 'repository' instead of 'library'
                //     ExecuteAddBook(library, choice); //repository.AddBook(pb)
                //     break;
                // case "2": // CHANGE REQUIRED: Pass 'repository' instead of 'library'
                //     ExecuteAddBook(library, choice); //repository.AddBook(db)
                //     break;
                // case "3": // CHANGE REQUIRED: Pass 'repository' instead of 'library'
                //     ExecuteAddBook(library, choice); //repository.AddBook(ab)
                //     break;

                // case "4": // CHANGE REQUIRED: Change to 'repository.PrintAllBooks()' or iterate over 'repository.GetAllBooks()'
                //     library.PrintAllBooks();
                //     break;
                // case "5": // CHANGE REQUIRED: Pass 'repository' instead of 'library'
                //     BorrowBookUI(library);
                //     break;
                // case "6": // CHANGE REQUIRED: Pass 'repository' instead of 'library'
                //     ReturnBookUI(library);
                //     break;
                // case "7": // CHANGE REQUIRED: Change to 'repository.DigitalBookReport()'
                //     library.DigitalBookReport();
                //     break;
                // case "8": // CHANGE REQUIRED: Change to 'repository.AudioBookReport()'
                //     library.AudioBookReport();
                //     break;
                // case "9": // CHANGE REQUIRED: Change to 'repository.PhysicalBookReport()'
                //     library.PhysicalBookReport();
                //     break;
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


















    // CHANGE REQUIRED: Change signature parameter 
    // from 'Library library' to 'IBookRepository repository'


    // OLD method static void ExecuteAddBook(Library library, string user_choice)


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
                    // CHANGE REQUIRED: Change 'library.AddBook(pb)' to 'repository.AddBook(pb)'
                    // OLD if (library.AddBook(pb))
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
                    // CHANGE REQUIRED: Change 'library.AddBook(db)' to 'repository.AddBook(db)'
                    // OLD if (Library.AddBook(db))
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
                    // CHANGE REQUIRED: Change 'library.AddBook(ab)' to 'repository.AddBook(ab)'
                    //OLD  if (library.AddBook(ab))
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
























    // NOT ORIGINAL - NEED TO CHECK REQUEST WORD !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1 

    // Prints all items coming out of your modern List/Array GetAll database query
    static void PrintAllBooksUI(IBookRepository repository)
    {
        Book[] bk = repository.GetAll();
        foreach (var item in bk)
        {
            item.PrintInfo();
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
            Console.WriteLine("Book not found.");
            return;
        }

        if (book is PhysicalBook pb)
        {
            if (pb.Borrow()) // update total -= 1
            {
                if (repository.Update(pb))
                {
                    Console.WriteLine("Book borrowed successfully!");
                }
                else
                {
                    Console.WriteLine(DatabaseErrorMessage);
                }
            }
            else
            {
                Console.WriteLine(DatabaseErrorMessage);
            }
        }
        else
        {
            Console.WriteLine(DatabaseErrorMessage);
        }
    }



    // CHANGE REQUIRED: Change signature parameter from
    //  'Library library' to 'IBookRepository repository'
    // static void BorrowBookUI(Library library)
    // {
    //     string idInput = GetInput("Enter Book ID: ");

    //     if (!TryReadNonNegativeInt(idInput, out int id))
    //     {
    //         Console.WriteLine(InvalidInputMessage);
    //         return;
    //     }




    //     // CHANGE REQUIRED: Change 'library.BorrowBook(id)' to 'repository.BorrowBook(id)'
    //     if (library.BorrowBook(id))
    //     {
    //         Console.WriteLine("Book borrowed successfully");
    //     }
    //     else
    //     {
    //         Console.WriteLine(DatabaseErrorMessage);
    //     }
    // }



















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
            Console.WriteLine("Book not found.");
            return;
        }

        if (book is PhysicalBook pb)
        {
            pb.Return(); // Increment tracking count back up
            if (repository.Update(pb))
            {
                Console.WriteLine("Book returned successfully!");
            }
            else
            {
                Console.WriteLine(DatabaseErrorMessage);
            }
        }
        else
        {
            Console.WriteLine(DatabaseErrorMessage);
        }
    }

    // CHANGE REQUIRED: Change signature parameter 
    // from 'Library library' to 'IBookRepository repository'
    // static void ReturnBookUI(Library library)
    // {
    //     string idInput = GetInput("Enter Book ID: ");

    //     if (!TryReadNonNegativeInt(idInput, out int id))
    //     {
    //         Console.WriteLine(InvalidInputMessage);
    //         return;
    //     }

    //     // CHANGE REQUIRED: Change 'library.ReturnBook(id)'
    //     //  to 'repository.ReturnBook(id)'
    //     if (library.ReturnBook(id))
    //     {
    //         Console.WriteLine("Book returned successfully");
    //     }
    //     else
    //     {
    //         Console.WriteLine(DatabaseErrorMessage);
    //     }
    // }












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






    // Dynamic clean client report sorting strategy pattern
    // static void ExecuteReportUI(IBookRepository repository, string filterType)
    // {
    //     Book[] allBooks = repository.GetAll();
    //     Console.WriteLine($"\n=== {filterType} BOOKS REPORT ===");
    //     int count = 0;

    //     foreach (var book in allBooks)
    //     {
    //         if ((filterType == "PHYSICAL" && book is PhysicalBook) ||
    //             (filterType == "DIGITAL" && book is DigitalBook) ||
    //             (filterType == "AUDIO" && book is AudioBook))
    //         {
    //             Console.WriteLine(book.ToString());
    //             count++;
    //         }
    //     }
    //     Console.WriteLine($"Total match inventory count: {count}");
    // }
}
