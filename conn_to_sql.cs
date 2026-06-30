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

                // string insertSql = "INSERT INTO cars (name, price, year) VALUES (@name, @price, @year);";
                // using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                // {
                //     // Parameterized assignments preventing SQL Injection vulnerabilities
                //     cmd.Parameters.AddWithValue("@name", "Toyota Yaris");
                //     cmd.Parameters.AddWithValue("@price", 96000);
                //     cmd.Parameters.AddWithValue("@year", 2016);
                //     
                //     int rowsInserted = cmd.ExecuteNonQuery();
                //     Console.WriteLine($"[INSERT] Added 'Toyota Yaris' (2016). Rows affected: {rowsInserted}");
                // }


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
                string selectSql = "SELECT id, name, price, year FROM cars;"; // update the SELECT query to TABLE
                using (MySqlCommand cmd = new MySqlCommand(selectSql, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n=============================================================");
                    // Fixed-width alignment configurations to format console tabular display
                    Console.WriteLine($"| {"ID",-5} | {"Car Name",-20} | {"Price (NIS)",-12} | {"Year",-6} |"); // Edit the TABLE titles for nice print
                    Console.WriteLine("-------------------------------------------------------------");

                    while (reader.Read())
                    {
                        int id = reader.GetInt32("id");
                        string name = reader.GetString("name");
                        int price = reader.GetInt32("price");
                        int year = reader.GetInt32("year");

                        // Printing structured table line using standard thousands separators (:N0)
                        Console.WriteLine($"| {id,-5} | {name,-20} | {price,-12:N0} | {year,-6} |");
                    }
                    Console.WriteLine("=============================================================");
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