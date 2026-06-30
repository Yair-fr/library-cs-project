using System;


/*
 *  Project Authors: Yair Friedensohn & Omri Vigodman
 */



// Application entry point
public class Program
{

    // new flag for error
    const string InvalidInputMessage = "Invalid input. Please try again.";
    public static void Main(string[] args)
    {
        Library library = new Library();
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
                    ExecuteAddBook(library, choice);
                    break;
                case "2":
                    ExecuteAddBook(library, choice);
                    break;
                case "3":
                    ExecuteAddBook(library, choice);
                    break;
                case "4":
                    library.PrintAllBooks();
                    break;
                case "5":
                    BorrowBookUI(library);
                    break;
                case "6":
                    ReturnBookUI(library);
                    break;
                case "7":
                    library.DigitalBookReport();
                    break;
                case "8":
                    library.AudioBookReport();
                    break;
                case "9":
                    library.PhysicalBookReport();
                    break;
                case "10":
                    running = false;
                    break;
            }
        }
    }



    // Replace for GetInt INT 
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

    // static int GetInt(string prompt)
    // { //                   maybe here print the error msg isted of 'prompt'
    //     return int.TryParse(GetInput(prompt), out int value) ? value : GetInt("Invalid input. Please try again."); // if value <= 0 than "Invalid input. Please try again."
    // }

    // static double GetDouble(string prompt)
    // {
    //     return double.TryParse(GetInput(prompt), out double value) ? value : GetDouble(prompt);
    // }

    // Unified DRY helper for adding books
    static void ExecuteAddBook(Library library, string user_choice)
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
                    library.AddBook(pb);
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
                    library.AddBook(db);
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
                    library.AddBook(ab);
                    break;
                }
        }
    }

    static void BorrowBookUI(Library library)
    {
        string idInput = GetInput("Enter Book ID: ");

        if (!TryReadNonNegativeInt(idInput, out int id))
        {
            Console.WriteLine(InvalidInputMessage);
            return;
        }

        if (library.BorrowBook(id))
        {
            Console.WriteLine("Book borrowed successfully");
        }
        else
        {
            Console.WriteLine("Error, book not borrowed");
        }
    }

    static void ReturnBookUI(Library library)
    {
        string idInput = GetInput("Enter Book ID: ");

        if (!TryReadNonNegativeInt(idInput, out int id))
        {
            Console.WriteLine(InvalidInputMessage);
            return;
        }

        if (library.ReturnBook(id))
        {
            Console.WriteLine("Book returned successfully");
        }
        else
        {
            Console.WriteLine("Error, book not returned");
        }
    }
}
