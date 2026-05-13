using System;
using Microsoft.Data.SqlClient;

namespace IndustrialEquipmentRentalSystem.Forms.Delete
{
    public class DeleteSafetyInspection
    {
        public static int DeleteByAgreement(int agreementID)
        {

            string query = "DELETE FROM SafetyInspection WHERE AgreementID = @AgreementID";
            
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AgreementID", agreementID);
                
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}