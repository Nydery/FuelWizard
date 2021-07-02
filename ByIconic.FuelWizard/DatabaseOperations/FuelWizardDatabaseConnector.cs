using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByIconic.FuelWizard.DatabaseOperations
{
    class FuelWizardDatabaseConnector
    {
        private static MySqlConnection conn = null;
        private static MySqlCommand sqlCommand = null;

        //Unsafe, bc not threadsafe
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
    }
}
