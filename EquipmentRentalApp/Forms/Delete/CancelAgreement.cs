
using System;
using Microsoft.Data.SqlClient;

namespace IndustrialEquipmentRentalSystem.Forms.Delete
{
    public class CancelAgreement
    {
        public static bool Cancel(int agreementID)
        {
            // Ahmed Maher's transaction code
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.ConnectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                
                try
                {
                    // Delete SafetyInspections first (FK dependency)
                    SqlCommand deleteInspections = new SqlCommand(
                        "DELETE FROM SafetyInspection WHERE AgreementID = @AgreementID",
                        conn, transaction);
                    deleteInspections.Parameters.AddWithValue("@AgreementID", agreementID);
                    deleteInspections.ExecuteNonQuery();
                    
                    // Then delete RentalAgreement
                    SqlCommand deleteAgreement = new SqlCommand(
                        "DELETE FROM RentalAgreement WHERE AgreementID = @AgreementID",
                        conn, transaction);
                    deleteAgreement.Parameters.AddWithValue("@AgreementID", agreementID);
                    deleteAgreement.ExecuteNonQuery();
                    
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}