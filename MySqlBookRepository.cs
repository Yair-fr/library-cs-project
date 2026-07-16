using MySql.Data.MySqlClient;

public class MySqlBookRepository : IBookRepository
{
    // Check user and password
    private string _connectionString;

    // Constructor
    public MySqlBookRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool Add(Book book)
    {
        string insertSql = "INSERT INTO books (id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator) " +
                            "VALUES (@id, @title, @author, @type, @pages, @available_copies, @file_size_mb, @format, @duration_minutes, @narrator)";

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
                return false; // Unknown book type or null, abort execution safely
        }

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
            {
                conn.Open();

                cmd.Parameters.AddWithValue("@id", book.Id);
                cmd.Parameters.AddWithValue("@title", book.Title);
                cmd.Parameters.AddWithValue("@author", book.Author);
                cmd.Parameters.AddWithValue("@type", type);

                cmd.Parameters.Add("@pages", MySqlDbType.Int32).Value = pages.HasValue ? pages.Value : DBNull.Value;
                cmd.Parameters.Add("@available_copies", MySqlDbType.Int32).Value = copies.HasValue ? copies.Value : DBNull.Value;
                cmd.Parameters.Add("@file_size_mb", MySqlDbType.Double).Value = size.HasValue ? size.Value : DBNull.Value;
                cmd.Parameters.Add("@format", MySqlDbType.VarChar).Value = format != null ? format : DBNull.Value;
                cmd.Parameters.Add("@duration_minutes", MySqlDbType.Int32).Value = duration.HasValue ? duration.Value : DBNull.Value;
                cmd.Parameters.Add("@narrator", MySqlDbType.VarChar).Value = narrator != null ? narrator : DBNull.Value;

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected == 1;
            }
        }
        catch (MySqlException)
        {
            return false;
        }
    }

    public Book? GetById(int id)
    {
        string selectSql = "SELECT id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator " +
                            "FROM books WHERE id = @id";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(selectSql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return CreateBookFromReader(reader);
                    }
                }

                return null;
            }
        }
        catch (MySqlException)
        {
            return null;
        }
    }











    public int GetTotalBooksCount()
    {
        string countSql = "SELECT COUNT(*) FROM books";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString + "123"))
            using (MySqlCommand cmd = new MySqlCommand(countSql, conn))
            {
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // Check if a row exists (COUNT query always returns exactly one row)
                    if (reader.Read())
                    {
                        // Retrieve the count from the first column (index 0) as an integer
                        return reader.GetInt32(0);
                    }
                }

                return 0;
            }
        }
        catch (MySqlException)
        {
            // Return -1 if a database error occurs
            return -1;
        }
    }


















    public Book[] GetAll()
    {
        // Book[] books = new Book[100];
        // Get the actual total rows from the database to initialize the array with the exact size
        int totalRows = GetTotalBooksCount();

        switch (totalRows)
        {
            case -1: return new Book[2];
            case 0: totalRows++; break;
            default: break;
        }

        Book[] books = new Book[totalRows];
        int currentIndex = 0;


        string selectSql = "SELECT id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator FROM books";

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
                        Book? currentBook = CreateBookFromReader(reader);

                        if (currentBook != null)
                        {
                            books[currentIndex] = currentBook;
                            currentIndex++;
                        }
                    }
                }
            }
        }
        catch (MySqlException)
        {
            return new Book[1];
        }

        if (currentIndex > 0)
        {

            // Return an array containing only the loaded books
            Book[] result = new Book[currentIndex];

            for (int i = 0; i < currentIndex; i++)
            {
                result[i] = books[i];
            }

            return result;
        }
        else
        {
            return new Book[1];
        }
    }






    public bool Update(Book book)
    {
        try
        {
            if (book is PhysicalBook pb)
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                using (MySqlCommand cmd = new MySqlCommand(
                    "UPDATE books SET available_copies = @available_copies WHERE id = @id", conn))
                {
                    // Clean syntax
                    cmd.Parameters.Add("@available_copies", MySqlDbType.Int32).Value = pb.AvailableCopies;
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = pb.Id;
                    conn.Open();
                    return cmd.ExecuteNonQuery() == 1;
                }
            }

            // DigitalBook and AudioBook do not change when borrowed/returned.
            return true;
        }
        catch (MySqlException)
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand("DELETE FROM books WHERE id = @id", conn))
            {
                // Explicitly set as a 32-bit Integer for absolute type consistency
                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                // cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
        catch (MySqlException)
        {
            return false;
        }
    }

    private Book? CreateBookFromReader(MySqlDataReader reader)
    {
        // These fields are required for all books and are never NULL
        int id = reader.GetInt32("id");
        string title = reader.GetString("title");
        string author = reader.GetString("author");
        string type = reader.GetString("type");

        // Use standard switch pattern mapping to reconstruct the exact subclass
        switch (type.ToUpper())
        {
            case "PHYSICAL":
                // Safely read columns only if they are not DBNull
                int pages = !reader.IsDBNull(reader.GetOrdinal("pages")) ? reader.GetInt32("pages") : 0;
                int copies = !reader.IsDBNull(reader.GetOrdinal("available_copies")) ? reader.GetInt32("available_copies") : 0;
                return new PhysicalBook(id, title, author, pages, copies);

            case "DIGITAL":
                double size = !reader.IsDBNull(reader.GetOrdinal("file_size_mb")) ? reader.GetDouble("file_size_mb") : 0.0;
                string format = !reader.IsDBNull(reader.GetOrdinal("format")) ? reader.GetString("format") : "";
                return new DigitalBook(id, title, author, size, format);

            case "AUDIO":
                int duration = !reader.IsDBNull(reader.GetOrdinal("duration_minutes")) ? reader.GetInt32("duration_minutes") : 0;
                string narrator = !reader.IsDBNull(reader.GetOrdinal("narrator")) ? reader.GetString("narrator") : "";
                return new AudioBook(id, title, author, duration, narrator);

            default:
                return null; // Unknown book type in database safely ignored
        }
    }
}
