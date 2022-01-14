using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class Get : Connection, Domain.Interfaces.DBOperations.IDBGet
    {
        public DataTable PublicMessages()
        {
            DataTable dataTable = new DataTable();

            string query = "SELECT * FROM PublicChat";

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                adapter.Fill(dataTable);

                command.ExecuteNonQuery();
                connection.Close();

                return dataTable;
            }
        }
        public byte[] Salt(string name)
        {
            byte[] salt = null;

            string query = "SELECT Salt FROM UserData WHERE UserData.Name = @Name";

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
                    salt = (byte[])item["Salt"];
                }

                command.ExecuteNonQuery();
                connection.Close();

                return salt;
            }
        }
        public int UserGender(string name)
        {
            int sex = 0;

            string query = "SELECT Sex FROM UserData WHERE UserData.Name = @Name";

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
                    sex = (byte)item["Sex"];
                }

                command.ExecuteNonQuery();
                connection.Close();

                return sex;
            }
        }
    }
}
