using ByIconic.FuelWizard.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByIconic.FuelWizard.DatabaseOperations
{
    class FuelWizardDatabaseConnector
    {
        public static string Host { get; set; }
        public static int Port { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string Database { get; set; }

        private static MySqlConnection conn = null;
        private static MySqlCommand sqlCommand = null;

        //Unsafe, because not threadsafe
        // Need to think of a way to ensure reliability and performance of this class and its function


        private static void Connect(string host, int port, string user, string pass, string database)
        {
            StringBuilder connStringBuilder = new StringBuilder();
            connStringBuilder.Append("server=");
            connStringBuilder.Append(host);

            connStringBuilder.Append(";port=");
            connStringBuilder.Append(port);

            connStringBuilder.Append(";uid=");
            connStringBuilder.Append(user);

            connStringBuilder.Append(";pwd=");
            connStringBuilder.Append(pass);

            connStringBuilder.Append(";database=");
            connStringBuilder.Append(database);

            conn = new MySqlConnection(connStringBuilder.ToString());
            conn.Open();

            sqlCommand = conn.CreateCommand();
        }

        private static void Disconnect()
        {
            conn.Close();
        }

        internal static Location[] GetLocations()
        {
            List<Location> result = new List<Location>();

            Connect(Host, Port, Username, Password, Database);

            sqlCommand.CommandText = "select * from cities;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                Location location = new Location();
                location.Name = reader[0].ToString();
                location.Longitude = Convert.ToInt32(reader[1]);
                location.Latitude = Convert.ToInt32(reader[2]);

                result.Add(location);
            }

            reader.Close();
            Disconnect();

            return result.ToArray();
        }
    }
}
