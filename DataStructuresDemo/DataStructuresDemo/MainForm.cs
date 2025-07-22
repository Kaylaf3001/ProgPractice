using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DataStructuresDemo.Models;

namespace DataStructuresDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeDataStructuresComboBox();
        }

        private void InitializeDatabase()
        {
            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataStructuresComboBox()
        {
            // Dictionary to store data structure names and their descriptions
            var dataStructures = new Dictionary<string, string>
            {
                // List<T>: A dynamic array that can grow in size automatically
                // Best for: Collections that need frequent additions/removals and index-based access
                {"List<User>", "Dynamic array that can grow in size. Fast access by index, good for frequent additions/removals at the end."},
                
                // Array: Fixed-size collection of elements
                // Best for: When you know the exact number of elements and need better performance
                {"Array", "Fixed-size collection. Fastest access by index but size cannot be changed after creation."},
                
                // Multi-dimensional Array: Table-like structure with rows and columns
                // Best for: Grid-based data, matrices, or tabular data
                {"Multi-dimensional Array", "Table-like structure (e.g., 2D arrays). Good for grid-based data with fixed dimensions."},
                
                // Jagged Array: Array of arrays with varying lengths
                // Best for: When you need arrays of different lengths within the same structure
                {"Jagged Array", "Array of arrays where each element can be a different size. More flexible than multi-dimensional arrays."},
                
                // Dictionary<TKey, TValue>: Key-value pair collection
                // Best for: Fast lookups by key, when you need to find items by a unique identifier
                {"Dictionary<int, User>", "Key-value pairs. Extremely fast lookups by key (O(1)). Each key must be unique."},
                
                // Queue<T>: First-In-First-Out (FIFO) collection
                // Best for: Processing items in the order they were added (like a waiting line)
                {"Queue<User>", "FIFO (First-In-First-Out) collection. Use when you need to process items in the order they were added."},
                
                // Stack<T>: Last-In-First-Out (LIFO) collection
                // Best for: When you need to process items in reverse order (like undo operations)
                {"Stack<User>", "LIFO (Last-In-First-Out) collection. The last item added is the first one to be removed."},
                
                // HashSet<T>: Collection of unique elements with fast lookups
                // Best for: When you need to ensure uniqueness and perform set operations
                {"HashSet<User>", "Collection of unique elements. Very fast for checking if an item exists (O(1)). No duplicate items allowed."},
                
                // LinkedList<T>: Collection where each element points to the next one
                // Best for: Frequent insertions/deletions in the middle of the collection
                {"LinkedList<User>", "Doubly-linked list. Fast insertions/deletions anywhere in the list, but slower index-based access."}
            };

            // Clear existing items
            cboDataStructures.Items.Clear();
            
            // Add items with descriptions
            foreach (var kvp in dataStructures)
            {
                cboDataStructures.Items.Add(new { Display = $"{kvp.Key} - {kvp.Value}", Value = kvp.Key });
            }
            
            // Set display and value members
            cboDataStructures.DisplayMember = "Display";
            cboDataStructures.ValueMember = "Value";
            
            // Select first item by default
            if (cboDataStructures.Items.Count > 0)
                cboDataStructures.SelectedIndex = 0;
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    using (var connection = DatabaseHelper.GetConnection())
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO Users (FirstName, LastName, Email, Age) VALUES (@firstName, @lastName, @email, @age)";
                        
                        using (var command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                            command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                            command.Parameters.AddWithValue("@email", txtEmail.Text);
                            command.Parameters.AddWithValue("@age", Convert.ToInt32(txtAge.Text));
                            
                            command.ExecuteNonQuery();
                            MessageBox.Show("User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadAndDisplayData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                !int.TryParse(txtAge.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Please fill in all fields with valid data.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtAge.Clear();
            txtFirstName.Focus();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            LoadAndDisplayData();
        }

        private void LoadAndDisplayData()
        {
            try
            {
                txtOutput.Clear();
                var users = GetUsersFromDatabase();
                
                if (users.Count == 0)
                {
                    txtOutput.Text = "No users found in the database.";
                    return;
                }

                // Get the selected value from the combo box (the data structure name without description)
                if (cboDataStructures.SelectedItem != null)
                {
                    var selectedItem = (dynamic)cboDataStructures.SelectedItem;
                    string selectedValue = selectedItem.Value;

                    // Display the selected data structure with its description
                    txtOutput.Text = $"=== {selectedItem.Display} ===\r\n\r\n";
                    
                    switch (selectedValue)
                    {
                        case "List<User>":
                            DisplayList(users);
                            break;
                        case "Array":
                            DisplayArray(users.ToArray());
                            break;
                        case "Multi-dimensional Array":
                            DisplayMultiDimensionalArray(users);
                            break;
                        case "Jagged Array":
                            DisplayJaggedArray(users);
                            break;
                        case "Dictionary<int, User>":
                            DisplayDictionary(users);
                            break;
                        case "Queue<User>":
                            DisplayQueue(new Queue<User>(users));
                            break;
                        case "Stack<User>":
                            DisplayStack(new Stack<User>(users));
                            break;
                        case "HashSet<User>":
                            DisplayHashSet(new HashSet<User>(users));
                            break;
                        case "LinkedList<User>":
                            DisplayLinkedList(new LinkedList<User>(users));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<User> GetUsersFromDatabase()
        {
            var users = new List<User>();
            
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Users";
                
                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Age = Convert.ToInt32(reader["Age"])
                        });
                    }
                }
            }
            
            return users;
        }

        #region Display Methods for Different Data Structures

        private void DisplayList(List<User> users)
        {
            txtOutput.Text = "=== List<User> ===\r\n";
            foreach (var user in users)
            {
                txtOutput.AppendText($"- {user}\r\n");
            }
        }

        private void DisplayArray(User[] users)
        {
            txtOutput.Text = "=== Array ===\r\n";
            for (int i = 0; i < users.Length; i++)
            {
                txtOutput.AppendText($"[{i}]: {users[i]}\r\n");
            }
        }

        private void DisplayMultiDimensionalArray(List<User> users)
        {
            // Create a 2D array with 2 columns: ID and User
            var multiArray = new object[users.Count, 2];
            
            for (int i = 0; i < users.Count; i++)
            {
                multiArray[i, 0] = users[i].Id;
                multiArray[i, 1] = users[i];
            }

            txtOutput.Text = "=== Multi-dimensional Array (ID, User) ===\r\n";
            for (int i = 0; i < multiArray.GetLength(0); i++)
            {
                txtOutput.AppendText($"ID: {multiArray[i, 0]}, User: {multiArray[i, 1]}\r\n");
            }
        }

        private void DisplayJaggedArray(List<User> users)
        {
            // Create a jagged array where each row has a different number of columns
            var jaggedArray = new User[users.Count][];
            
            for (int i = 0; i < users.Count; i++)
            {
                // Each row has i+1 elements (1, 2, 3, ...)
                jaggedArray[i] = new User[i + 1];
                for (int j = 0; j <= i && j < users.Count; j++)
                {
                    jaggedArray[i][j] = users[j];
                }
            }

            txtOutput.Text = "=== Jagged Array ===\r\n";
            for (int i = 0; i < jaggedArray.Length; i++)
            {
                txtOutput.AppendText($"Row {i + 1}: ");
                for (int j = 0; j < jaggedArray[i].Length; j++)
                {
                    txtOutput.AppendText($"{jaggedArray[i][j].FirstName[0]}.{jaggedArray[i][j].LastName[0]} ");
                }
                txtOutput.AppendText("\r\n");
            }
        }

        private void DisplayDictionary(List<User> users)
        {
            var dict = new Dictionary<int, User>();
            foreach (var user in users)
            {
                dict[user.Id] = user;
            }

            txtOutput.Text = "=== Dictionary<int, User> ===\r\n";
            foreach (var kvp in dict)
            {
                txtOutput.AppendText($"Key: {kvp.Key}, Value: {kvp.Value}\r\n");
            }
        }

        private void DisplayQueue(Queue<User> queue)
        {
            txtOutput.Text = "=== Queue<User> (FIFO) ===\r\n";
            txtOutput.AppendText("Queue order (first to be dequeued first):\r\n");
            
            int count = 1;
            foreach (var user in queue)
            {
                txtOutput.AppendText($"{count++}. {user}\r\n");
            }
        }

        private void DisplayStack(Stack<User> stack)
        {
            txtOutput.Text = "=== Stack<User> (LIFO) ===\r\n";
            txtOutput.AppendText("Stack order (top to bottom):\r\n");
            
            int count = 1;
            foreach (var user in stack)
            {
                txtOutput.AppendText($"{count++}. {user}\r\n");
            }
        }

        private void DisplayHashSet(HashSet<User> hashSet)
        {
            txtOutput.Text = "=== HashSet<User> ===\r\n";
            txtOutput.AppendText($"Total unique users: {hashSet.Count}\r\n\r\n");
            
            foreach (var user in hashSet)
            {
                txtOutput.AppendText($"- {user}\r\n");
            }
        }

        private void DisplayLinkedList(LinkedList<User> linkedList)
        {
            txtOutput.Text = "=== LinkedList<User> ===\r\n";
            
            var current = linkedList.First;
            int index = 1;
            
            while (current != null)
            {
                txtOutput.AppendText($"Node {index++}: {current.Value}\r\n");
                current = current.Next;
            }
        }

        #endregion
    }
}
