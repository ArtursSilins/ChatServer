using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class Check : Connection, Domain.Interfaces.DBOperations.IDBCheck
    {
        /// <summary>
        /// Compare name and hashed password.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool UserExists(string name, byte[] hash)
        {
            string tempHash = "";
            string tempName = "";

            bool exists = false;

            string query = "SELECT Name, Hash FROM UserData WHERE UserData.Name = @Name AND UserData.Hash = @Hash";

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Hash", hash);
                adapter.Fill(dataTable);

                foreach (DataRow item in dataTable.Rows)
                {
                    tempName = Convert.ToString(item["Name"]);
                    tempHash = Convert.ToString(item["Hash"]);
                }

                if (tempHash.Equals(hash) || tempName.Equals(name)) exists = true;

                // if (tempHash != null) exists = true;

                command.ExecuteNonQuery();
                connection.Close();
            }
            return exists;
        }
        public bool UserNameExists(string name)
        {
            string tempName = "";

            bool exists = false;

            string query = "SELECT Name FROM UserData WHERE UserData.Name = @Name";

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Name", name);
                adapter.Fill(dataTable);

                foreach (DataRow item in dataTable.Rows)
                {
                    tempName = Convert.ToString(item["Name"]);
                }

                if (tempName.Equals(name)) exists = true;

                command.ExecuteNonQuery();
                connection.Close();
            }
            return exists;
        }
        public bool EmailExists(string email)
        {
            string tempName = "";

            bool exists = false;

            string query = "SELECT Email FROM UserData WHERE UserData.Email = @Email";

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Email", email);
                adapter.Fill(dataTable);

                foreach (DataRow item in dataTable.Rows)
                {
                    tempName = Convert.ToString(item["Email"]);
                }

                if (tempName.Equals(email)) exists = true;

                command.ExecuteNonQuery();
                connection.Close();
            }
            return exists;
        }
    }
}
