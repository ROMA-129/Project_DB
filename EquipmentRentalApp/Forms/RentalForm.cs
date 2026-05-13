using System;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using EquipmentRentalApp.Database;

namespace EquipmentRentalApp.Forms
{
    public partial class RentalForm : Form
    {
        private DataGridView dgvRentals;
        private ComboBox cboEquipment, cboContractor;
        private DateTimePicker dtpStart, dtpEnd;
        private Button btnCreate, btnDelete;
        private Label lblMsg;
        
        public RentalForm()
        {
            InitializeUI();
            LoadEquipment();
            LoadContractors();
            LoadRentals();
        }
        
        private void InitializeUI()
        {
            this.Text = "Rental Agreement Management";
            this.Size = new Size(1100, 650);
            this.BackColor = Color.FromArgb(45, 45, 50);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            Label lblTitle = new Label
            {
                Text = "📋 Rental Agreements",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(400, 40)
            };
            this.Controls.Add(lblTitle);
            
            // Input Panel
            Panel inputPanel = new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(1050, 160),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            Label lblEquipment = new Label { Text = "Equipment:", Location = new Point(20, 25), Size = new Size(100, 25), ForeColor = Color.White };
            cboEquipment = new ComboBox { Location = new Point(130, 23), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblContractor = new Label { Text = "Contractor:", Location = new Point(20, 65), Size = new Size(100, 25), ForeColor = Color.White };
            cboContractor = new ComboBox { Location = new Point(130, 63), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblStart = new Label { Text = "Start Date:", Location = new Point(450, 25), Size = new Size(100, 25), ForeColor = Color.White };
            dtpStart = new DateTimePicker { Location = new Point(560, 23), Size = new Size(200, 25), Format = DateTimePickerFormat.Short };
            
            Label lblEnd = new Label { Text = "End Date:", Location = new Point(450, 65), Size = new Size(100, 25), ForeColor = Color.White };
            dtpEnd = new DateTimePicker { Location = new Point(560, 63), Size = new Size(200, 25), Format = DateTimePickerFormat.Short };
            
            btnCreate = new Button { Text = "➕ Create Agreement", Location = new Point(800, 25), Size = new Size(160, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnCreate.Click += BtnCreate_Click;
            
            btnDelete = new Button { Text = "🗑 Cancel/Delete", Location = new Point(800, 70), Size = new Size(160, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblEquipment, cboEquipment, lblContractor, cboContractor,
                lblStart, dtpStart, lblEnd, dtpEnd,
                btnCreate, btnDelete
            });
            this.Controls.Add(inputPanel);
            
            // Message
            lblMsg = new Label { Location = new Point(20, 240), Size = new Size(500, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvRentals = new DataGridView
            {
                Location = new Point(20, 270),
                Size = new Size(1050, 340),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(dgvRentals);
        }
        
        private void LoadEquipment()
        {
            // FIX: correct table name "Equipment"
            string query = "SELECT EquipmentID, Model FROM Equipment WHERE Status = 'Available' ORDER BY Model";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboEquipment.DisplayMember = "Model";
            cboEquipment.ValueMember = "EquipmentID";
            cboEquipment.DataSource = dt;
        }
        
        private void LoadContractors()
        {
            // FIX: correct table name "Contractor"
            string query = "SELECT ContractorID, CompanyName FROM Contractor ORDER BY CompanyName";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboContractor.DisplayMember = "CompanyName";
            cboContractor.ValueMember = "ContractorID";
            cboContractor.DataSource = dt;
        }
        
        private void LoadRentals()
        {
            // FIX: correct table names "RentalAgreement", "Equipment", "Contractor"
            // FIX: removed non-existent "ReturnStatus" column - using actual columns StartDate, EndDate, TotalCost
            string query = @"SELECT r.AgreementID, e.Model AS Equipment, c.CompanyName AS Contractor,
                                    r.StartDate, r.EndDate, r.TotalCost
                             FROM RentalAgreement r
                             JOIN Equipment e ON e.EquipmentID = r.EquipmentID
                             JOIN Contractor c ON c.ContractorID = r.ContractorID
                             ORDER BY r.StartDate DESC";
            dgvRentals.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgvRentals.Columns["AgreementID"] != null)
                dgvRentals.Columns["AgreementID"].Visible = false;
        }
        
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (cboEquipment.SelectedValue == null || cboContractor.SelectedValue == null)
            {
                ShowMessage("Please select equipment and contractor.", Color.Red);
                return;
            }

            if (dtpEnd.Value <= dtpStart.Value)
            {
                ShowMessage("End date must be after start date.", Color.Red);
                return;
            }

            // FIX: correct table/column names, removed ReturnStatus (doesn't exist in DB)
            // Calculate TotalCost based on hours and hourly rate
            string query = @"INSERT INTO RentalAgreement (ContractorID, EquipmentID, StartDate, EndDate, TotalCost) 
                             VALUES (@cont, @eq, @start, @end,
                                (SELECT HourlyRate * DATEDIFF(HOUR, @start2, @end2) FROM Equipment WHERE EquipmentID = @eq2))";
            var parameters = new[]
            {
                new SqlParameter("@eq",    cboEquipment.SelectedValue),
                new SqlParameter("@cont",  cboContractor.SelectedValue),
                new SqlParameter("@start", dtpStart.Value.Date),
                new SqlParameter("@end",   dtpEnd.Value.Date),
                new SqlParameter("@start2", dtpStart.Value.Date),
                new SqlParameter("@end2",   dtpEnd.Value.Date),
                new SqlParameter("@eq2",   cboEquipment.SelectedValue)
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Rental agreement created!", Color.LightGreen);

                // Mark equipment as Rented
                string updateEq = "UPDATE Equipment SET Status = 'Rented' WHERE EquipmentID = @id";
                DatabaseHelper.ExecuteNonQuery(updateEq, new[] { new SqlParameter("@id", cboEquipment.SelectedValue) });

                LoadRentals();
                LoadEquipment();
            }
        }
        
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRentals.SelectedRows.Count == 0)
            {
                ShowMessage("Select an agreement to cancel.", Color.Red);
                return;
            }
            
            var confirm = MessageBox.Show("Cancel this rental agreement?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                int agreementId = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["AgreementID"].Value);

                // Get the equipment ID so we can free it up
                object eqIdObj = DatabaseHelper.ExecuteScalar(
                    "SELECT EquipmentID FROM RentalAgreement WHERE AgreementID = @id",
                    new[] { new SqlParameter("@id", agreementId) });

                string query = "DELETE FROM RentalAgreement WHERE AgreementID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", agreementId) });
                
                if (result > 0)
                {
                    // Mark equipment back to Available
                    if (eqIdObj != null)
                    {
                        string updateEq = "UPDATE Equipment SET Status = 'Available' WHERE EquipmentID = @id";
                        DatabaseHelper.ExecuteNonQuery(updateEq, new[] { new SqlParameter("@id", eqIdObj) });
                    }
                    ShowMessage("Agreement cancelled.", Color.Yellow);
                    LoadRentals();
                    LoadEquipment();
                }
            }
        }
        
        private void ShowMessage(string msg, Color color)
        {
            lblMsg.Text = msg;
            lblMsg.ForeColor = color;
        }
    }
}