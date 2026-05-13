using System;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace EquipmentRentalApp.Forms.Update
{
    public class UpdateContractor
    {
        private string connectionString;
        
        public UpdateContractor(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        
        public int UpdateCreditLimit(string companyName, decimal newLimit)
        {
            int rowsAffected = 0;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Contractor SET CreditLimit = @newLimit WHERE CompanyName = @compName";
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@newLimit", newLimit);
                cmd.Parameters.AddWithValue("@compName", companyName);
                
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            
            return rowsAffected;
        }
        
        // Optional: Update by ContractorID (more reliable)
        public int UpdateCreditLimitById(int contractorId, decimal newLimit)
        {
            int rowsAffected = 0;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Contractor SET CreditLimit = @newLimit WHERE ContractorID = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@newLimit", newLimit);
                cmd.Parameters.AddWithValue("@id", contractorId);
                
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            
            return rowsAffected;
        }
        
        // Update multiple fields
        public int UpdateContractorDetails(int contractorId, string companyName, string phone, decimal creditLimit)
        {
            int rowsAffected = 0;
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE Contractor 
                              SET CompanyName = @name, 
                                  Phone = @phone, 
                                  CreditLimit = @limit 
                              WHERE ContractorID = @id";
                
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", companyName);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@limit", creditLimit);
                cmd.Parameters.AddWithValue("@id", contractorId);
                
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            
            return rowsAffected;
        }
    }
}