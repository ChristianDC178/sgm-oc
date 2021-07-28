using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Update_FechaCierreProximo_DIARCO
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd");
            SqlConnection sqlConnection = null;

            try
            {
                string connStr = "Data Source=192.168.0.217;Persist Security Info=True;User ID=SA;Password=admin01";
                sqlConnection = new SqlConnection(connStr);

                List<SqlCommand> commands = new List<SqlCommand>();

                commands.Add(new SqlCommand($"UPDATE [DiarcoP].[dbo].[T900_FECHA_CIERRE_PROXIMO] SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP002.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP007.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP009.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP012.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP019.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP020.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP026.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP037.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP040.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP043.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP048.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));
                commands.Add(new SqlCommand($"UPDATE DIARCOP069.DBO.T900_FECHA_CIERRE_PROXIMO SET F_PROXIMO_CIERRE = @fechaCierre", sqlConnection));

                sqlConnection.Open();

                foreach (var item in commands)
                {
                    item.Parameters.AddWithValue("fechaCierre",DateTime.Now.ToString("yyyy - dd - MM"));
                    item.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {  
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}