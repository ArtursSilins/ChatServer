using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class Connection
    {
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename = C:\Users\artur\Documents\Visual Studio 2017\Projects\ChatServer\Repository\ChatDB.mdf;Integrated Security=True;");
        }
    }
}
