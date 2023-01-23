using Microsoft.Data.Sqlite;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace habit_tracker
{
    class Program
    {
        // Below is the document management system databse 
        static string connectionString = @"Data Source = document-Management-System.db";
        static void Main(string[] args)
        {
           //Start out by creating the SQL Lite Database
            using(var connection = new SqliteConnection(connectionString)) // creating a SQLConnection instance
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = 
                    @"CREATE TABLE IF NOT EXISTS documents (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DocNumber TEXT,
                        DocName TEXT, 
                        Rev TEXT,
                        RecordRetention REAL,
                        Date TEXT
                        )";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }

             GetUserInput();
            
        }

        static void  GetUserInput()
        {
            Console.Clear();  // Clears the console
            bool closeApp = false; // start with the variable as false
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application.");
                Console.WriteLine("Type 1 to View All Records.");
                Console.WriteLine("Type 2 to Insert Record.");
                Console.WriteLine("Type 3 to Delete Record.");
                Console.WriteLine("Type 4 to Update Record.");
                Console.WriteLine("------------------------------------------\n");
            
                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true; //closeApp variable goes from 
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;
                }
            
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"SELECT * FROM documents";

                List<Documents> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                        {
                            tableData.Add(
                                new Documents 
                                {
                                    Id = reader.GetInt32(0),
                                    DocNumber = reader.GetString(1),
                                    DocName = reader.GetString(2),
                                    Rev = reader.GetString(3),
                                    RecordRetention = reader.GetDecimal(4),
                                    Date = DateTime.ParseExact(reader.GetString(5), "dd-MM-yy", new CultureInfo("en-US"))
                                }
                            );;
                        }
                }

                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();
                

                Console.WriteLine("------------------------------------------\n");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.DocNumber} - {dw.DocName} - {dw.Rev} - {dw.RecordRetention} - {dw.Date.ToString("dd-MMM-yyyy")}");
                }
                Console.WriteLine("------------------------------------------\n");

            }
        }



        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

           
            var UserChoice = GetNumberInput();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"DELETE from documents WHERE Id = '{UserChoice}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {UserChoice} doesn't exist. \n\n");
                    Delete();
                }

            }

            Console.WriteLine($"\n\nRecord with Id {UserChoice} was deleted. \n\n");

            GetUserInput();

        }

        private static void Insert()
        {
            Console.WriteLine("Enter the Document Number.");
            string DocNumber = Console.ReadLine();

            Console.WriteLine("Enter the Document Name");
            string DocName = Console.ReadLine();

            Console.WriteLine("Enter the Rev of the Document");
            string Rev = Console.ReadLine();
            
            decimal RecordRetention = GetRetentionMonths("Enter the number of months for retention of the document.");

            string Date = GetDateRecord ();


            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                   $"INSERT INTO documents(DocNumber, DocName, Rev, RecordRetention, Date) VALUES('{DocNumber}', '{DocName}', '{Rev}', {RecordRetention}, '{Date}')";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }


        }

         public static decimal GetRetentionMonths(string message)
            {
                Console.WriteLine("Enter the number of months for retention of the document.");
                var stringmonths = Console.ReadLine();

                decimal decimalmonths = Convert.ToDecimal(stringmonths);

                return decimalmonths;
            }

        internal static string GetDateRecord()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main manu.\n\n");

            string dateInput = Console.ReadLine();
            return dateInput;

        }

        public static int GetNumberInput()
        {
             Console.WriteLine("Choose the ID of the record that you wish to delete.");
            var NumberString = Console.ReadLine();
            var NumberInt = Int32.Parse(NumberString);
            return NumberInt;
        }

        internal static void Update()
        {
            
            GetAllRecords();

            var Id = GetNumberInput();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM documents WHERE Id = {Id})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {Id} doesn't exist.\n\n");
                    connection.Close();
                    Update();
                }

               Console.WriteLine("Enter the Document Number.");
                string DocNumber = Console.ReadLine();

                Console.WriteLine("Enter the Document Name");
                string DocName = Console.ReadLine();

                Console.WriteLine("Enter the Rev of the Document");
                string Rev = Console.ReadLine();
                
                decimal RecordRetention = GetRetentionMonths("Enter the number of months for retention of the document.");

                string Date = GetDateRecord ();


                

                var tableCmd = connection.CreateCommand();



                tableCmd.CommandText = $"UPDATE documents SET DocNumber = '{DocNumber}', DocName = '{DocName}',  WHERE Id = {Id}";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
    }

       
    public class Documents
    {
        public int Id {get; set;}
        public string? DocNumber {get; set;}
        public string? DocName {get; set;}
        public string? Rev {get; set;}
        public decimal? RecordRetention{get; set;}
        public DateTime Date {get; set;}
    }
}