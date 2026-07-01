using System.Xml;
using MySql.Data.MySqlClient;

public class MySqlBookRepository : IBookRepository
{
    // Check user and password
    private string _connectionString = "server=localhost;database=library_db;user=root;password=admin;";


    public MySqlBookRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Empty constructor 
    public MySqlBookRepository() : this("server=localhost;database=library_db;user=root;password=admin;")
    {
    }

    public bool Add(Book book)
    {
        string insertSql = "INSERT INTO books (id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator) " +
                            "VALUES (@id, @title, @author, @type, @pages, @available_copies, @file_size_mb, @format, @duration_minutes, @narrator)";

        // Explicitly declaring local variables without using the 'var' keyword
        string type;
        int? pages;
        int? copies;
        double? size;
        string? format;
        int? duration;
        string? narrator;

        // Pattern matching switch statement to extract specific subclass properties safely
        switch (book)
        {
            case PhysicalBook p:
                type = "PHYSICAL";
                pages = p.Pages;
                copies = p.AvailableCopies;
                size = null;
                format = null;
                duration = null;
                narrator = null;
                break;

            case DigitalBook d:
                type = "DIGITAL";
                pages = null;
                copies = null;
                size = d.FileSizeMB;
                format = d.Format;
                duration = null;
                narrator = null;
                break;

            case AudioBook a:
                type = "AUDIO";
                pages = null;
                copies = null;
                size = null;
                format = null;
                duration = a.DurationMinutes;
                narrator = a.Narrator;
                break;

            default:
                return false; // Unknown book type, abort execution safely
        }

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
            {
                conn.Open();

                // Mapping shared global properties to SQL parameters
                cmd.Parameters.AddWithValue("@id", book.Id);
                cmd.Parameters.AddWithValue("@title", book.Title);
                cmd.Parameters.AddWithValue("@author", book.Author);

                // Mapping class-specific subclass variables prepared during the switch phase
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@pages", pages);
                cmd.Parameters.AddWithValue("@available_copies", copies);
                cmd.Parameters.AddWithValue("@file_size_mb", size);
                cmd.Parameters.AddWithValue("@format", format);
                cmd.Parameters.AddWithValue("@duration_minutes", duration);
                cmd.Parameters.AddWithValue("@narrator", narrator);

                // Executing command safely against target database instance
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (MySqlException ex)
        {
            // Error code 1062 represents MySQL Duplicate Entry / Primary Key Violation
            if (ex.Number == 1062)
            {
                Console.WriteLine($"[Database Error]: A book with ID {book.Id} already exists in the system.");
            }
            else
            {
                Console.WriteLine($"[Database Error]: Native MySQL exception occurred: {ex.Message}");
            }
            return false;
        }
    }


























    public Book? GetById(int id)
    {
        string selectSql = "SELECT id, title, author, type, pages, available_copies FROM books WHERE id = @id";
        string updateSql = "UPDATE books SET available_copies = @available_copies WHERE id = @id";

        // Variables to hold the fetched data outside the reader scope
        int bookId = 0;
        string title = "";
        string author = "";
        string type = "";
        int pages = 0;
        int currentCopies = 0;
        bool recordFound = false;

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // STEP 1: Find the book by ID using a data reader
                using (MySqlCommand selectCmd = new MySqlCommand(selectSql, conn))
                {
                    selectCmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            recordFound = true;
                            bookId = reader.GetInt32("id");
                            title = reader.GetString("title");
                            author = reader.GetString("author");
                            type = reader.GetString("type");
                            pages = reader.IsDBNull(reader.GetOrdinal("pages")) ? 0 : reader.GetInt32("pages");
                            currentCopies = reader.IsDBNull(reader.GetOrdinal("available_copies")) ? 0 : reader.GetInt32("available_copies");
                        }
                    } // The reader is explicitly closed here, freeing the connection for the UPDATE command
                }

                // If no match was found by ID, return null immediately
                if (!recordFound)
                {
                    Console.WriteLine($"[BORROW] Book with ID {id} was not found.");
                    return null;
                }

                // STEP 2: Only proceed with subtraction if the matched book is PHYSICAL
                if (type.ToUpper() == "PHYSICAL")
                {
                    if (currentCopies <= 0)
                    {
                        Console.WriteLine($"[BORROW] Failed. No physical copies available for ID: {id}");
                        return null;
                    }

                    int updatedCopies = currentCopies - 1;

                    // STEP 3: Execute the UPDATE command to set original value minus one
                    using (MySqlCommand updateCmd = new MySqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@id", id);
                        updateCmd.Parameters.AddWithValue("@available_copies", updatedCopies);

                        int rowsAffected = updateCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"[BORROW] Successfully updated available copies to {updatedCopies} for book ID: {id}");
                            return new PhysicalBook(bookId, title, author, pages, updatedCopies);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[BORROW] Match found by ID, but book type is '{type}' and cannot be physically borrowed.");
                    return null;
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"[Database Error]: Transaction failed. Native message: {ex.Message}");
            return null;
        }

        return null;
    }



    public Book[] GetAll()
    {
        // Start with a fixed-size array of 100 items as required
        Book[] books = new Book[100];
        int currentIndex = 0;

        string selectSql = "SELECT id, title, author, type, pages, available_copies FROM books";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(selectSql, conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // 1. Read global fields shared across all book variations
                        int bookId = reader.GetInt32("id");
                        string title = reader.GetString("title");
                        string author = reader.GetString("author");
                        string type = reader.GetString("type");
                        Book? currentBook = null;

                        // 2. Evaluate the concrete type and construct the exact matching subclass object
                        switch (type.ToUpper())
                        {
                            case "PHYSICAL":
                                int pages = reader.IsDBNull(reader.GetOrdinal("pages")) ? 0 : reader.GetInt32("pages");
                                int copies = reader.IsDBNull(reader.GetOrdinal("available_copies")) ? 0 : reader.GetInt32("available_copies");
                                currentBook = new PhysicalBook(bookId, title, author, pages, copies);
                                break;

                            // Add your DIGITAL and AUDIO case statements here if needed in the future
                            default:
                                continue; // Skip unrecognized types safely
                        }

                        if (currentBook != null)
                        {
                            // 3. Dynamically expand the fixed array size only if it runs out of pre-allocated slots
                            if (currentIndex >= books.Length)
                            {
                                Array.Resize(ref books, books.Length * 2);
                            }

                            // Assign the structured object to the active index and advance the pointer
                            books[currentIndex] = currentBook;
                            currentIndex++;
                        }
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"[Database Error]: Failed to populate book array. Native message: {ex.Message}");
        }

        // 4. Clean up the final array to trim unused trailing null padding slots exactly to the current index count
        Array.Resize(ref books, currentIndex);
        return books;
    }


    public bool Update(Book book)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            using (MySqlCommand cmd = new MySqlCommand("UPDATE Books SET Title = @Title, Author = @Author WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@Author", book.Author);
                cmd.Parameters.AddWithValue("@Id", book.Id);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public bool Delete(int id)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            using (MySqlCommand cmd = new MySqlCommand("DELETE FROM Books WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }


}
