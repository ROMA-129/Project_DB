using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace EquipmentRentalApp
{
    public partial class SelectTableGUI : Form
    {
        Microsoft.Data.SqlClient.SqlConnection con = new Microsoft.Data.SqlClient.SqlConnection(
        "Server=localhost;Database=EquipmentRentalDB;Trusted_Connection=True;");

        private Button btnLoad;
        private DataGridView dataGridView1;

        public SelectTableGUI()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnLoad = new Button();
            this.dataGridView1 = new DataGridView();

            this.SuspendLayout();

            // btnLoad
            this.btnLoad.Location = new System.Drawing.Point(12, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(100, 30);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new EventHandler(this.btnLoad_Click);

            // dataGridView1
            this.dataGridView1.Location = new System.Drawing.Point(12, 50);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(760, 400);
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.TabIndex = 1;

            // SelectTableGUI
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.dataGridView1);
            this.Name = "SelectTableGUI";
            this.Text = "Select Table";

            this.ResumeLayout(false);
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