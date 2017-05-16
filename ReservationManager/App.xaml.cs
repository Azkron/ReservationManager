using PRBD_Framework;
using ReservationManager.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReservationManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string MSG_NEW_MEMBER = "MSG_NEW_MEMBER", MSG_CLIENT_CHANGED = "MSG_CLIENT_CHANGED", MSG_SHOW_CLIENT = "MSG_SHOW_CLIENT",
                        MSG_SHOW_MEMBER = "MSG_SHOW_MEMBER", MSG_CLOSE_TAB = "MSG_CLOSE_TAB";

        public static Messenger Messenger { get; } = new Messenger();

        public static Entities Model { get; private set; } = new Entities();

        public static User CurrentUser { get; set; }

        public App()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Culture);
            PrepareDatabase();
            ColdStart();
            //TestingEntityFramework();
        }

        private void ColdStart()
        {
            Model.Users.Find(1000);
        }

        public static void CancelChanges()
        {
            Model.Dispose();
            Model = new Entities();
            Model.Database.Log = m => Console.Write(m);
        }

        private void PrepareDatabase()
        {
            // give a value to the property "DataProperty" that is used as base directory in App.config for the
            // connection string to the DB. This value is calculated in relative path from the folder with the
            // executable, so <project folder>/bin/Debug
            var projectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")); // @ allows to use \ without \\
            var dbPath = Path.GetFullPath(Path.Combine(projectPath, @"database"));
            Console.WriteLine("Database path: " + dbPath);
            AppDomain.CurrentDomain.SetData("DataDirectory", projectPath);

            // if the database does not exist, create it by executing the .sql
            if (!File.Exists(Path.Combine(dbPath, "ReservationManager.mdf")))
            {
                Console.WriteLine(dbPath);
                Console.WriteLine("Creating database...");
                string script = File.ReadAllText(Path.Combine(dbPath, "ReservationManager.sql"));

                // in the script, we replace "{DBPATH}" for teh folder where we want to create the DB
                script = script.Replace("{DBPATH}", dbPath);

                // we split the content of the script in a list of strings, each one containing an sql command.
                // to do this split, we use the GO commands as delimiters
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                // We connect to the driver of the data base "(lobaldb)\MSSQLocalDB" that allows to work with 
                // attached SQL Server files without needing for an instance of SQL Server to be present
                string sqlConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=true;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True";
                SqlConnection connection = new SqlConnection(sqlConnectionString);
                connection.Open();

                // we execute the SQL commands 1 by 1
                foreach (string commandString in commandStrings)
                    if (commandString.Trim() != "")
                        using (var command = new SqlCommand(commandString, connection))
                            command.ExecuteNonQuery();
            }
        }
    }


}
