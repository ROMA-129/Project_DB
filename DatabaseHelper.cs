using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public static class DatabaseHelper
    {
        // *** Change this connection string to match your SQL Server ***
        private static readonly string ConnectionString =
            @"Server=localhost;Database=EquipmentRentalDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch { return false; }
        }

        public static DataTable ExecuteQuery(string sql, SqlParameter[] parameters = null)
        {
            var dt = new DataTable();
            try
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    using (var adapter = new SqlDataAdapter(cmd))
                        adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static object ExecuteScalar(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
