using ByIconic.FuelWizard.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using static ByIconic.FuelWizard.Models.API.EControlClasses;

namespace ByIconic.FuelWizard.DatabaseOperations
{
    class FuelWizardDatabaseConnector
    {
        public static string Host { get; set; } = "dev.byiconic.at";
        public static int Port { get; set; } = 3306;
        public static string Username { get; set; } = "fuelwizard_datacollector";
        public static string Password { get; set; } = "@DosfaOg*saFGJ?";
        public static string Database { get; set; } = "fuelwizard";

        //Unsafe, because not threadsafe
        // Need to think of a way to ensure reliability and performance of this class and its function


        private static void Connect(string host, int port, string user, string pass, string database, out MySqlConnection conn, out MySqlCommand sqlCommand)
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

        internal static IEnumerable<Location> GetLocations()
        {
            List<Location> result = new List<Location>();

            Connect(Host, Port, Username, Password, Database, out MySqlConnection conn, out MySqlCommand sqlCommand);

            sqlCommand.CommandText = "select * from cities;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                Location location = new Location();
                location.address = reader[0].ToString();
                location.longitude = float.Parse(reader[1].ToString());
                location.latitude = float.Parse(reader[2].ToString());

                result.Add(location);
            }

            reader.Close();
            conn.Close();

            return result.ToArray();
        }

        public static void InsertPriceData(int gasStationId, string fuelType, DateTime dateTime, double value)
        {
            Connect(Host, Port, Username, Password, Database, out MySqlConnection conn, out MySqlCommand sqlCommand);

            string val = value.ToString().Replace(',', '.');

            sqlCommand.CommandText = $"insert into prices (gasstationId, fuelType, datetime, value) values ({gasStationId}, '{fuelType}', '{dateTime}', {val})";  
            sqlCommand.ExecuteNonQuery();

            conn.Close();
        }

        internal static bool ExistsGasStation(int id)
        {
            bool result = false;

            Connect(Host, Port, Username, Password, Database, out MySqlConnection conn, out MySqlCommand sqlCommand);

            sqlCommand.CommandText = $"select * from gasstations where id={id}";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.Read())
                result = true;

            reader.Close();

            return result;
        }

        internal static void InsertGasStation(GasStationPublic gasStation)
        {
            Connect(Host, Port, Username, Password, Database, out MySqlConnection conn, out MySqlCommand sqlCommand);

            string longitude = gasStation.location.longitude.ToString().Replace(',', '.');
            string latitude = gasStation.location.latitude.ToString().Replace(',', '.');

            sqlCommand.CommandText = $"insert into gasstations (id, name, longitude, latitude) values({gasStation.id}, '{gasStation.name}', {longitude}, {latitude})";
            sqlCommand.ExecuteNonQuery();

            conn.Close();
        }
    }
}
