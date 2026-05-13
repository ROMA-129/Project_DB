using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EquipmentRentalApp
{
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=RentalProject;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnUpdateEquipment_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Equipment SET HourlyRate = @newRate WHERE Model LIKE @modelSearch";
                SqlCommand cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@newRate", txtNewRate.Text);
                cmd.Parameters.AddWithValue("@modelSearch", "%Caterpillar%");

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show($"{rows} Records Updated");
            }
        }

        private void btnUpdateContractor_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Contractor SET CreditLimit = @newLimit WHERE CompanyName = @compName";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@newLimit", txtCreditLimit.Text);
                cmd.Parameters.AddWithValue("@compName", "Apex Construction LLC");

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show($"{rows} Records Updated");
            }
        }
    }
}