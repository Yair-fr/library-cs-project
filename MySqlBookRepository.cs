using MySql.Data.MySqlClient;

public class MySqlBookRepository : IBookRepository
{
    private string _connectionString;

    public MySqlBookRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool Add(Book book)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Books (Title, Author) VALUES (@Title, @Author)", conn))
            {
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@Author", book.Author);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }

    public Book? GetById(int id)
    {
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Books WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;

                    return new Book(
                        reader.GetInt32("Id"),
                        reader.GetString("Title"),
                        reader.GetString("Author")
                    );
                }
            }
        }
    }

    public Book[] GetAll()
    {
        Book[] books = new Book[100]; // like Task_1 size = 100
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Books", conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Book book = new Book(
                            reader.GetInt32("Id"),
                            reader.GetString("Title"),
                            reader.GetString("Author")
                        );

                        Array.Resize(ref books, books.Length + 1);
                        books[books.Length - 1] = book;
                    }
                }
            }
        }
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
