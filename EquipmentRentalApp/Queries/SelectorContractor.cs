using System;
using Microsoft.Data.SqlClient;
using EquipmentRentalApp.Database;

namespace EquipmentRentalApp.Queries
{
    public class SelectContractor
    {
        private string connectionString;
        
        public SelectContractor(string connectionString)
        {
            this.connectionString = connectionString;
        }
        
        public void GetContractors()
        {
            string query = "SELECT * FROM Contractor";
            
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    Console.WriteLine(
                        reader["ContractorID"] + " " +
                        reader["CompanyName"] + " " +
                        reader["CreditLimit"]
                    );
                }
                con.Close();
            }
        }
    }
}