using System;
using MySql.Data.MySqlClient;

class conn_to_sql
{
    public static void Sub()
    {
        // Updated connection string for your cars_DB_cs local database instance
        string connStr = "server=localhost;database=library_db;user=root;password=admin;"; // check DB name

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                // Open database connection
                conn.Open();
                Console.WriteLine("=== [SUCCESS] Connected to MySQL Server ===");
                Console.WriteLine("-------------------------------------------------------------\n");

                // ==========================================
                // 1. ALTER TABLE Command (Modify schema structure)
                // ==========================================
                // try
                // {
                //     string alterSql = "ALTER TABLE cars ADD COLUMN year INT DEFAULT 2020;";
                //     using (MySqlCommand cmd = new MySqlCommand(alterSql, conn))
                //     {
                //         cmd.ExecuteNonQuery();
                //         Console.WriteLine("[ALTER] Column 'year' added successfully.");
                //     }
                // }
                // catch (MySqlException ex) when (ex.Number == 1060) // Error 1060: Column already exists
                // {
                //     Console.WriteLine("[ALTER] Notice: Column 'year' already exists. Skipping structural change.");
                // }



                // ==========================================
                // 2. INSERT Command (Secure parameterized data insertion)
                // ==========================================

                string insertSql = "INSERT INTO books (id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator) " +
                                    "VALUES (@id, @title, @author, @type, @pages, @available_copies, @file_size_mb, @format, @duration_minutes, @narrator)";

                using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                {
                    // SQL Injection
                    cmd.Parameters.AddWithValue("@id", 1);
                    cmd.Parameters.AddWithValue("@title", "The Great Gatsby");
                    cmd.Parameters.AddWithValue("@author", "F. Scott Fitzgerald");
                    cmd.Parameters.AddWithValue("@type", "PHYSICAL");
                    cmd.Parameters.AddWithValue("@pages", 180);
                    cmd.Parameters.AddWithValue("@available_copies", 5);

                    // Null values
                    cmd.Parameters.AddWithValue("@file_size_mb", DBNull.Value);
                    cmd.Parameters.AddWithValue("@format", DBNull.Value);
                    cmd.Parameters.AddWithValue("@duration_minutes", DBNull.Value);
                    cmd.Parameters.AddWithValue("@narrator", DBNull.Value);

                    int rowsInserted = cmd.ExecuteNonQuery();
                    Console.WriteLine($"[INSERT] Added '{cmd.Parameters["@title"].Value}'. Rows affected: {rowsInserted}");
                }



                // ==========================================
                // 3. DELETE Command (Removing targeted rows safely)
                // ==========================================

                // Clean database table from low-priced vehicle entries
                // string deleteSql = "DELETE FROM cars WHERE price < @price";
                // using (MySqlCommand cmd = new MySqlCommand(deleteSql, conn))
                // {
                //     cmd.Parameters.AddWithValue("@price", 15000);
                //     
                //     int rowsDeleted = cmd.ExecuteNonQuery();
                //     Console.WriteLine($"[DELETE] Cleaned up cheap database records. Rows removed: {rowsDeleted}");
                // }


                // ==========================================
                // 4. SELECT Command & Formatted Grid Display
                // ==========================================
                // Select all columns from the books table to match all insert parameters
                string selectSql = "SELECT id, title, author, type, pages, available_copies, file_size_mb, format, duration_minutes, narrator FROM books;";

                using (MySqlCommand cmd = new MySqlCommand(selectSql, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n=================================================================================================================================================");
                    // Fixed-width alignment configurations to format console tabular display for all 10 columns
                    Console.WriteLine($"| {"ID",-4} | {"Title",-22} | {"Author",-18} | {"Type",-8} | {"Pages",-5} | {"Copies",-6} | {"Size(MB)",-8} | {"Format",-6} | {"Min",-5} | {"Narrator",-15} |");
                    Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------");

                    while (reader.Read())
                    {
                        // 1. Read non-nullable columns directly
                        int id = reader.GetInt32("id");
                        string title = reader.GetString("title");
                        string author = reader.GetString("author");
                        string type = reader.GetString("type");

                        // 2. Handle all nullable columns safely using IsDBNull to prevent runtime exceptions
                        string pages = reader.IsDBNull(reader.GetOrdinal("pages")) ? "N/A" : reader.GetInt32("pages").ToString();
                        string copies = reader.IsDBNull(reader.GetOrdinal("available_copies")) ? "N/A" : reader.GetInt32("available_copies").ToString();
                        string fileSize = reader.IsDBNull(reader.GetOrdinal("file_size_mb")) ? "N/A" : reader.GetDouble("file_size_mb").ToString("0.0");
                        string format = reader.IsDBNull(reader.GetOrdinal("format")) ? "N/A" : reader.GetString("format");
                        string duration = reader.IsDBNull(reader.GetOrdinal("duration_minutes")) ? "N/A" : reader.GetInt32("duration_minutes").ToString();
                        string narrator = reader.IsDBNull(reader.GetOrdinal("narrator")) ? "N/A" : reader.GetString("narrator");

                        // 3. Print the perfectly structured table row
                        Console.WriteLine($"| {id,-4} | {title,-22} | {author,-18} | {type,-8} | {pages,-5} | {copies,-6} | {fileSize,-8} | {format,-6} | {duration,-5} | {narrator,-15} |");
                    }
                    Console.WriteLine("=================================================================================================================================================");
                }


                conn.Close();
            }
        }
        catch (MySqlException ex)
        {
            // Evaluate specific target code identifiers derived from native MySqlException.Number evaluations
            switch (ex.Number)
            {
                case 0:
                    Console.WriteLine("\n[Database Error]: Cannot connect to server. Please check if MySQL Service is running.");
                    break;

                case 1045:
                    Console.WriteLine("\n[Database Error]: Invalid username or password. Connection denied.");
                    break;

                case 1049:
                    Console.WriteLine("\n[Database Error]: Database schema not found. Please verify the database name.");
                    break;

                case 1062:
                    Console.WriteLine("\n[Database Error]: Duplicate primary key violation. This record already exists.");
                    break;

                case 1451:
                case 1452:
                    Console.WriteLine("\n[Database Error]: Foreign key constraint violation. Integrity check failed.");
                    break;

                default:
                    Console.WriteLine($"\n[Database Error]: {ex.Message} (Error Code: {ex.Number})");
                    break;
            }
        }
        catch (Exception ex)
        {
            // Global catching backup block for application-level runtime failures (e.g., Cast failures, NullPointers)
            Console.WriteLine($"\n[General Application Error]: {ex.Message}");
        }
    }
}