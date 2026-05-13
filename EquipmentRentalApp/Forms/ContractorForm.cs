using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using EquipmentRentalApp.Database;
using Microsoft.Data.SqlClient;    

namespace EquipmentRentalApp.Forms
{
    public partial class ContractorForm : Form
    {
        private DataGridView dgvContractors;
        private TextBox txtCompanyName, txtContactInfo, txtCreditLimit;
        private Button btnInsert, btnUpdate, btnDelete, btnClear;
        private Label lblMsg;
        
        public ContractorForm()
        {
            InitializeUI();
            LoadContractors();
        }
        
        private void InitializeUI()
        {
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(45, 45, 50);
            
            // Title
            Label lblTitle = new Label
            {
                Text = "Contractor Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(400, 40)
            };
            this.Controls.Add(lblTitle);
            
            // Input Panel
            Panel inputPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(850, 150),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            // Company Name
            Label lblCompany = new Label { Text = "Company Name:", Location = new Point(20, 20), Size = new Size(120, 25), ForeColor = Color.White };
            txtCompanyName = new TextBox { Location = new Point(150, 18), Size = new Size(250, 25) };
            
            // Contact Info
            Label lblContact = new Label { Text = "Contact Info:", Location = new Point(20, 55), Size = new Size(120, 25), ForeColor = Color.White };
            txtContactInfo = new TextBox { Location = new Point(150, 53), Size = new Size(250, 25) };
            
            // Credit Limit
            Label lblCredit = new Label { Text = "Credit Limit:", Location = new Point(20, 90), Size = new Size(120, 25), ForeColor = Color.White };
            txtCreditLimit = new TextBox { Location = new Point(150, 88), Size = new Size(150, 25) };
            
            // Buttons
            btnInsert = new Button { Text = "➕ Add", Location = new Point(500, 18), Size = new Size(100, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnUpdate = new Button { Text = "✏ Update", Location = new Point(610, 18), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnUpdate.Click += BtnUpdate_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(720, 18), Size = new Size(100, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            btnClear = new Button { Text = "Clear", Location = new Point(720, 65), Size = new Size(100, 30), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };
            btnClear.Click += (s, e) => ClearForm();
            
            inputPanel.Controls.AddRange(new Control[] { lblCompany, txtCompanyName, lblContact, txtContactInfo, lblCredit, txtCreditLimit, btnInsert, btnUpdate, btnDelete, btnClear });
            this.Controls.Add(inputPanel);
            
            // Message Label
            lblMsg = new Label { Location = new Point(20, 240), Size = new Size(400, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvContractors = new DataGridView
            {
                Location = new Point(20, 270),
                Size = new Size(850, 300),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvContractors.SelectionChanged += DgvContractors_SelectionChanged;
            this.Controls.Add(dgvContractors);
        }
        
        private void LoadContractors()
        {
            string query = "SELECT ContractorID, CompanyName, ContactInfo, CreditLimit FROM Contractor ORDER BY CompanyName";
            dgvContractors.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgvContractors.Columns["ContractorID"] != null)
                dgvContractors.Columns["ContractorID"].Visible = false;
        }
        
        private void DgvContractors_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvContractors.SelectedRows.Count > 0)
            {
                var row = dgvContractors.SelectedRows[0];
                txtCompanyName.Text = row.Cells["CompanyName"].Value?.ToString();
                txtContactInfo.Text = row.Cells["ContactInfo"].Value?.ToString();
                txtCreditLimit.Text = row.Cells["CreditLimit"].Value?.ToString();
            }
        }
        
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                DatabaseHelper.ShowError("Company name is required.");
                return;
            }
            
            if (!decimal.TryParse(txtCreditLimit.Text, out decimal creditLimit))
                creditLimit = 0;
            
            string query = @"INSERT INTO Contractor (CompanyName, ContactInfo, CreditLimit) 
                             VALUES (@name, @contact, @credit)";
            var parameters = new[]
            {
                new SqlParameter("@name", txtCompanyName.Text.Trim()),
                new SqlParameter("@contact", txtContactInfo.Text.Trim()),
                new SqlParameter("@credit", creditLimit)
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                DatabaseHelper.ShowSuccess("Contractor added successfully!");
                LoadContractors();
                ClearForm();
            }
        }
        
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvContractors.SelectedRows.Count == 0)
            {
                DatabaseHelper.ShowError("Select a contractor to update.");
                return;
            }
            
            int id = Convert.ToInt32(dgvContractors.SelectedRows[0].Cells["ContractorID"].Value);
            
            string query = @"UPDATE Contractor SET CompanyName = @name, ContactInfo = @contact, CreditLimit = @credit 
                             WHERE ContractorID = @id";
            var parameters = new[]
            {
                new SqlParameter("@name", txtCompanyName.Text.Trim()),
                new SqlParameter("@contact", txtContactInfo.Text.Trim()),
                new SqlParameter("@credit", decimal.TryParse(txtCreditLimit.Text, out decimal c) ? c : 0),
                new SqlParameter("@id", id)
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                DatabaseHelper.ShowSuccess("Contractor updated successfully!");
                LoadContractors();
            }
        }
        
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvContractors.SelectedRows.Count == 0)
            {
                DatabaseHelper.ShowError("Select a contractor to delete.");
                return;
            }
            
            var confirm = MessageBox.Show("Delete this contractor?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvContractors.SelectedRows[0].Cells["ContractorID"].Value);
                string query = "DELETE FROM Contractor WHERE ContractorID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", id) });
                
                if (result > 0)
                {
                    DatabaseHelper.ShowSuccess("Contractor deleted successfully!");
                    LoadContractors();
                    ClearForm();
                }
            }
        }
        
        private void ClearForm()
        {
            txtCompanyName.Clear();
            txtContactInfo.Clear();
            txtCreditLimit.Clear();
            lblMsg.Text = "";
        }
    }
}