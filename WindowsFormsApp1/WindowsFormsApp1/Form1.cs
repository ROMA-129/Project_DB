using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace IndustrialEquipmentProject
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection(
        "Server=localhost;Database=IndustrialEquipmentDB;Trusted_Connection=True;");

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da =
            new SqlDataAdapter("SELECT * FROM Technician", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }
    }
}
