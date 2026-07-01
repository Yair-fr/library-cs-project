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

                cmd.Parameters.AddWithValue("@id", book.Id);
                cmd.Parameters.AddWithValue("@title", book.Title);
                cmd.Parameters.AddWithValue("@author", book.Author);
                cmd.Parameters.AddWithValue("@type", type);

                // For columns that do not belong to the current book type, send DBNull.Value to MySQL
                cmd.Parameters.AddWithValue("@pages", pages.HasValue ? (object)pages.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@available_copies", copies.HasValue ? (object)copies.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@file_size_mb", size.HasValue ? (object)size.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@format", format != null ? (object)format : DBNull.Value);
                cmd.Parameters.AddWithValue("@duration_minutes", duration.HasValue ? (object)duration.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@narrator", narrator != null ? (object)narrator : DBNull.Value);

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

    public Book[] GetAll()
    {
        Book[] books = new Book[100];
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
                            if (currentIndex >= books.Length)
                            {
                                Array.Resize(ref books, books.Length * 2);
                            }

                            books[currentIndex] = currentBook;
                            currentIndex++;
                        }
                    }
                }
            }
        }
        catch (MySqlException)
        {
            return new Book[0];
        }

        Array.Resize(ref books, currentIndex);
        return books;
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
                    cmd.Parameters.AddWithValue("@available_copies", pb.AvailableCopies);
                    cmd.Parameters.AddWithValue("@id", pb.Id);

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
                cmd.Parameters.AddWithValue("@id", id);

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
        int id = reader.GetInt32("id");
        string title = reader.GetString("title");
        string author = reader.GetString("author");
        string type = reader.GetString("type").ToUpper();

        if (type == "PHYSICAL")
        {
            int pages = reader.IsDBNull(reader.GetOrdinal("pages")) ? 0 : reader.GetInt32("pages");
            int copies = reader.IsDBNull(reader.GetOrdinal("available_copies")) ? 0 : reader.GetInt32("available_copies");

            return new PhysicalBook(id, title, author, pages, copies);
        }

        if (type == "DIGITAL")
        {
            double fileSize = reader.IsDBNull(reader.GetOrdinal("file_size_mb")) ? 0.0 : reader.GetDouble("file_size_mb");
            string format = reader.IsDBNull(reader.GetOrdinal("format")) ? "" : reader.GetString("format");

            return new DigitalBook(id, title, author, fileSize, format);
        }

        if (type == "AUDIO")
        {
            int duration = reader.IsDBNull(reader.GetOrdinal("duration_minutes")) ? 0 : reader.GetInt32("duration_minutes");
            string narrator = reader.IsDBNull(reader.GetOrdinal("narrator")) ? "" : reader.GetString("narrator");

            return new AudioBook(id, title, author, duration, narrator);
        }

        return null;
    }
}
