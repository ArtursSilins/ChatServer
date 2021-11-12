using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DBOperationsRepository : Connection
    {
        public void Add(string userId, string name, string email, string salt, string hash)
        {
            string query = "INSERT INTO UserData VALUES (@PeronId, @Name, @Email, @Salt, @Hash )";

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@PersonId", userId);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Salt", salt);
                command.Parameters.AddWithValue("@Hash", hash);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public string GetHash(string userId)
        {
            string hash = "";

            string query = "SELECT PersonId FROM UserData WHERE UserData.PersonId = @PersonId";

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@PersonId", userId);
                adapter.Fill(dataTable);

                foreach (DataRow item in dataTable.Rows)
                {
                    hash = Convert.ToString(item["Hash"]);
                }

                command.ExecuteNonQuery();
                connection.Close();
            }
            return hash;
        }
    }
}
