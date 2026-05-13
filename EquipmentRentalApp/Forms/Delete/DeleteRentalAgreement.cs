
using System;
using Microsoft.Data.SqlClient;

namespace EquipmentRentalApp.Forms.Delete
{
    public class DeleteRentalAgreement
    {
        public static int Delete(int agreementID)
        {
            
            string query = "DELETE FROM RentalAgreement WHERE AgreementID = @AgreementID";
            
            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EquipmentRentalDB;Integrated Security=True"))

            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AgreementID", agreementID);
                
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}