using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DBOperationsRepository : Connection
    {
        //public void AddUser(string name, int sex, bool defautlPic, string email, byte[] salt, byte[] hash)
        //{
        //    string query = "INSERT INTO UserData VALUES (@Name, @Sex, @DefaultPic, @Email, @Salt, @Hash )";

        //    using (SqlConnection connection = GetSqlConnection())
        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //    {
        //        connection.Open();
        //        command.Parameters.AddWithValue("@Name", name);
        //        command.Parameters.AddWithValue("@Sex", sex);
        //        command.Parameters.AddWithValue("@DefaultPic", defautlPic);
        //        command.Parameters.AddWithValue("@Email", email);
        //        command.Parameters.AddWithValue("@Salt", salt);
        //        command.Parameters.AddWithValue("@Hash", hash);
        //        try
        //        {
        //        command.ExecuteNonQuery();

        //        }
        //        catch (Exception ex)
        //        {

        //            throw;
        //        }
        //        connection.Close();
        //    }
        //}

        //public void AddPublicMessage(string dateAndTime, string userID,
        //    string message, byte[] messageImage, string defautlPicture)
        //{
        //    string query = "INSERT INTO PublicChat VALUES (@DateAndTime, @UserId," +
        //                  " @Message, @MessageImage, @DefaultPicture )";

        //    using (SqlConnection connection = GetSqlConnection())
        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //    {
        //        connection.Open();
        //        command.Parameters.AddWithValue("@DateAndTime", dateAndTime);
        //        command.Parameters.AddWithValue("@UserId", userID);
        //        command.Parameters.AddWithValue("@Message", message);

        //        if (messageImage != null)
        //            command.Parameters.AddWithValue("@MessageImage", messageImage);
        //        else
        //            command.Parameters.AddWithValue("@MessageImage", SqlDbType.VarBinary).Value = SqlBinary.Null;

        //        command.Parameters.AddWithValue("@DefaultPicture", defautlPicture);
        //        try
        //        {
        //            command.ExecuteNonQuery();

        //        }
        //        catch (Exception ex)
        //        {

        //            throw;
        //        }
        //        connection.Close();
        //    }
        //}

        //public DataTable GetPublicMessages()
        //{
        //    DataTable dataTable = new DataTable();

        //    string query = "SELECT * FROM PublicChat";

        //    using (SqlConnection connection = GetSqlConnection())
        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //    {
        //        connection.Open();
        //        adapter.Fill(dataTable);

        //        command.ExecuteNonQuery();
        //        connection.Close();

        //        return dataTable;
        //    }
        //}
        //public byte[] GetSalt(string name)
        //{
        //    byte[] salt = null;

        //    string query = "SELECT Salt FROM UserData WHERE UserData.Name = @Name";

        //    DataTable dataTable = new DataTable();

        //    using (SqlConnection connection = GetSqlConnection())
        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //    {
        //        connection.Open();
        //        command.Parameters.AddWithValue("@Name", name);
        //        adapter.Fill(dataTable);

        //        foreach (DataRow item in dataTable.Rows)
        //        {
        //            salt = (byte[])item["Salt"];
        //        }

        //        command.ExecuteNonQuery();
        //        connection.Close();

        //        return salt;
        //    }
        //}
        //public int GetUserGender(string name)
        //{
        //    int sex = 0;

        //    string query = "SELECT Sex FROM UserData WHERE UserData.Name = @Name";

        //    DataTable dataTable = new DataTable();

        //    using (SqlConnection connection = GetSqlConnection())
        //    using (SqlCommand command = new SqlCommand(query, connection))
        //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //    {
        //        connection.Open();
        //        command.Parameters.AddWithValue("@Name", name);
        //        adapter.Fill(dataTable);

        //        foreach (DataRow item in dataTable.Rows)
        //        {
        //            sex = (byte)item["Sex"];
        //        }

        //        command.ExecuteNonQuery();
        //        connection.Close();

        //        return sex;
        //    }
        //}
        /// <summary>
        /// Compare name and hashed password.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool CheckIfUserExists(string name, byte[] hash)
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
        public bool CheckIfUserNameExists(string name)
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
    }
}
