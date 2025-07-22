using System;
using System.Data.SQLite;
using System.IO;

namespace DataStructuresDemo
{
    public static class DatabaseHelper
    {
        private static string databasePath = Path.Combine(Environment.CurrentDirectory, "UserData.db");
        private static string connectionString = $"Data Source={databasePath};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    
                    string createTableQuery = @"
                        CREATE TABLE Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT NOT NULL,
                            LastName TEXT NOT NULL,
                            Email TEXT NOT NULL UNIQUE,
                            Age INTEGER NOT NULL
                        )";
                    
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    // Insert sample data
                    InsertSampleData(connection);
                }
            }
        }

        private static void InsertSampleData(SQLiteConnection connection)
        {
            string insertQuery = @"
                INSERT OR IGNORE INTO Users (FirstName, LastName, Email, Age) VALUES 
                ('John', 'Doe', 'john.doe@example.com', 30),
                ('Jane', 'Smith', 'jane.smith@example.com', 25),
                ('Robert', 'Johnson', 'robert.j@example.com', 35),
                ('Emily', 'Williams', 'emily.w@example.com', 28),
                ('Michael', 'Brown', 'michael.b@example.com', 42)";

            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
