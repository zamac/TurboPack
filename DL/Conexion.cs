using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DL
{
    public class Conexion
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["TurboPack"].ConnectionString.ToString(); 
        }
        public static SqlCommand CreateComand(string Query, SqlConnection context)
        {
            context.Open();
            SqlCommand cmd = new SqlCommand(Query, context);
            return cmd;
        }
        public static int ExecuteComand(SqlCommand cmd)
        {
            return cmd.ExecuteNonQuery();
        }
        public static DataTable ExecuteComandSelect(SqlCommand cmd)
        {
            DataTable talbe = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(talbe);
            return talbe;
        }
    }
}
