using System;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace EquipmentRentalApp.Forms.Update
{
    public class UpdateEquipment
    {
        private string connectionString;
        
        public UpdateEquipment(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        
        public int UpdateHourlyRate(string modelSearch, decimal newRate)
        {
            int rowsAffected = 0;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Equipment SET HourlyRate = @newRate WHERE Model LIKE @modelSearch";
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@newRate", newRate);
                cmd.Parameters.AddWithValue("@modelSearch", "%" + modelSearch + "%");
                
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            
            return rowsAffected;
        }
        
        // Optional: More flexible update method
        public int UpdateEquipmentField(int equipmentId, string columnName, object newValue)
        {
            int rowsAffected = 0;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Equipment SET {columnName} = @newValue WHERE EquipmentID = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@newValue", newValue);
                cmd.Parameters.AddWithValue("@id", equipmentId);
                
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            
            return rowsAffected;
        }
    }
}