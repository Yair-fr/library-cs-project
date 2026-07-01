using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public class MySqlBookRepository : IBookRepository
{
    private string _connectionString;

    public MySqlBookRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MySqlBookRepository()
        : this("server=localhost;database=library_db;user=root;password=admin;")
    {
    }

    public bool Add(Book book)
    {
        if (book == null)
        {
            return false;
        }

        string sql = @"INSERT INTO books
            (id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator)
            VALUES
            (@id, @title, @author, @type, @pages, @available_copies, @file_size_mb, @format, @duration_minutes, @narrator)";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                if (!AddBookParameters(cmd, book))
                {
                    return false;
                }

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
        catch (MySqlException)
        {
            return false;
        }
    }

    public Book? GetById(int id)
    {
        string sql = @"SELECT id, title, author, type, pages, available_copies,
                              file_size_mb, format, duration_minutes, narrator
                       FROM books
                       WHERE id = @id";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
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
        List<Book> books = new List<Book>();

        string sql = @"SELECT id, title, author, type, pages, available_copies,
                              file_size_mb, format, duration_minutes, narrator
                       FROM books";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Book? book = CreateBookFromReader(reader);

                        if (book != null)
                        {
                            books.Add(book);
                        }
                    }
                }
            }
        }
        catch (MySqlException)
        {
            return new Book[0];
        }

        return books.ToArray();
    }

    public bool Update(Book book)
    {
        if (book == null)
        {
            return false;
        }

        string sql = @"UPDATE books SET
            title = @title,
            author = @author,
            type = @type,
            pages = @pages,
            available_copies = @available_copies,
            file_size_mb = @file_size_mb,
            format = @format,
            duration_minutes = @duration_minutes,
            narrator = @narrator
            WHERE id = @id";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                if (!AddBookParameters(cmd, book))
                {
                    return false;
                }

                conn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }
        catch (MySqlException)
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        string sql = "DELETE FROM books WHERE id = @id";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
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

    private bool AddBookParameters(MySqlCommand cmd, Book book)
    {
        cmd.Parameters.AddWithValue("@id", book.Id);
        cmd.Parameters.AddWithValue("@title", book.Title);
        cmd.Parameters.AddWithValue("@author", book.Author);

        if (book is PhysicalBook physical)
        {
            cmd.Parameters.AddWithValue("@type", "PHYSICAL");
            cmd.Parameters.AddWithValue("@pages", physical.Pages);
            cmd.Parameters.AddWithValue("@available_copies", physical.AvailableCopies);
            cmd.Parameters.AddWithValue("@file_size_mb", DBNull.Value);
            cmd.Parameters.AddWithValue("@format", DBNull.Value);
            cmd.Parameters.AddWithValue("@duration_minutes", DBNull.Value);
            cmd.Parameters.AddWithValue("@narrator", DBNull.Value);
            return true;
        }

        if (book is DigitalBook digital)
        {
            cmd.Parameters.AddWithValue("@type", "DIGITAL");
            cmd.Parameters.AddWithValue("@pages", DBNull.Value);
            cmd.Parameters.AddWithValue("@available_copies", DBNull.Value);
            cmd.Parameters.AddWithValue("@file_size_mb", digital.FileSizeMB);
            cmd.Parameters.AddWithValue("@format", digital.Format);
            cmd.Parameters.AddWithValue("@duration_minutes", DBNull.Value);
            cmd.Parameters.AddWithValue("@narrator", DBNull.Value);
            return true;
        }

        if (book is AudioBook audio)
        {
            cmd.Parameters.AddWithValue("@type", "AUDIO");
            cmd.Parameters.AddWithValue("@pages", DBNull.Value);
            cmd.Parameters.AddWithValue("@available_copies", DBNull.Value);
            cmd.Parameters.AddWithValue("@file_size_mb", DBNull.Value);
            cmd.Parameters.AddWithValue("@format", DBNull.Value);
            cmd.Parameters.AddWithValue("@duration_minutes", audio.DurationMinutes);
            cmd.Parameters.AddWithValue("@narrator", audio.Narrator);
            return true;
        }

        return false;
    }

    private Book? CreateBookFromReader(MySqlDataReader reader)
    {
        int id = reader.GetInt32(reader.GetOrdinal("id"));
        string title = reader.GetString(reader.GetOrdinal("title"));
        string author = reader.GetString(reader.GetOrdinal("author"));
        string type = reader.GetString(reader.GetOrdinal("type")).ToUpper();

        if (type == "PHYSICAL")
        {
            int pages = GetIntOrZero(reader, "pages");
            int copies = GetIntOrZero(reader, "available_copies");

            return new PhysicalBook(id, title, author, pages, copies);
        }

        if (type == "DIGITAL")
        {
            double fileSize = GetDoubleOrZero(reader, "file_size_mb");
            string format = GetStringOrEmpty(reader, "format");

            return new DigitalBook(id, title, author, fileSize, format);
        }

        if (type == "AUDIO")
        {
            int duration = GetIntOrZero(reader, "duration_minutes");
            string narrator = GetStringOrEmpty(reader, "narrator");

            return new AudioBook(id, title, author, duration, narrator);
        }

        return null;
    }

    private int GetIntOrZero(MySqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(index))
        {
            return 0;
        }

        return reader.GetInt32(index);
    }

    private double GetDoubleOrZero(MySqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(index))
        {
            return 0.0;
        }

        return reader.GetDouble(index);
    }

    private string GetStringOrEmpty(MySqlDataReader reader, string columnName)
    {
        int index = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(index))
        {
            return "";
        }

        return reader.GetString(index);
    }
}
