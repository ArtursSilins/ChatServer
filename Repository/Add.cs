using Repository.Interfaces;
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
    public class Add : Connection, Domain.Interfaces.DBOperations.IDBAdd
    {
        public void User(string name, int sex, bool defautlPic, string email, byte[] salt, byte[] hash)
        {
            string query = "INSERT INTO UserData VALUES (@Name, @Sex, @DefaultPic, @Email, @Salt, @Hash )";

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Sex", sex);
                command.Parameters.AddWithValue("@DefaultPic", defautlPic);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Salt", salt);
                command.Parameters.AddWithValue("@Hash", hash);
                try
                {
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    throw;
                }
                connection.Close();
            }
        }
        public void PublicMessage(string dateAndTime, string userID,
           string message, byte[] messageImage, string defautlPicture)
        {
            string query = "INSERT INTO PublicChat VALUES (@DateAndTime, @UserId," +
                          " @Message, @MessageImage, @DefaultPicture )";

            using (SqlConnection connection = GetSqlConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                command.Parameters.AddWithValue("@DateAndTime", dateAndTime);
                command.Parameters.AddWithValue("@UserId", userID);
                command.Parameters.AddWithValue("@Message", message);

                if (messageImage != null)
                    command.Parameters.AddWithValue("@MessageImage", messageImage);
                else
                    command.Parameters.AddWithValue("@MessageImage", SqlDbType.VarBinary).Value = SqlBinary.Null;

                command.Parameters.AddWithValue("@DefaultPicture", defautlPicture);
                try
                {
                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                    throw;
                }
                connection.Close();
            }
        }
    }
}
