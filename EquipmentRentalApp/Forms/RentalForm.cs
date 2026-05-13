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
        private Button btnCreate, btnReturn, btnDelete;
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
                Size = new Size(1050, 180),
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
            
            btnReturn = new Button { Text = "🔄 Mark Returned", Location = new Point(800, 70), Size = new Size(160, 35), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnReturn.Click += BtnReturn_Click;
            
            btnDelete = new Button { Text = "🗑 Cancel/Delete", Location = new Point(800, 115), Size = new Size(160, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblEquipment, cboEquipment, lblContractor, cboContractor,
                lblStart, dtpStart, lblEnd, dtpEnd,
                btnCreate, btnReturn, btnDelete
            });
            this.Controls.Add(inputPanel);
            
            // Message
            lblMsg = new Label { Location = new Point(20, 260), Size = new Size(500, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvRentals = new DataGridView
            {
                Location = new Point(20, 290),
                Size = new Size(1050, 320),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(dgvRentals);
        }
        
        private void LoadEquipment()
        {
            string query = "SELECT EquipmentID, Model FROM EQUIPMENT WHERE Status = 'Available' ORDER BY Model";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboEquipment.DisplayMember = "Model";
            cboEquipment.ValueMember = "EquipmentID";
            cboEquipment.DataSource = dt;
        }
        
        private void LoadContractors()
        {
            string query = "SELECT ContractorID, CompanyName FROM CONTRACTOR ORDER BY CompanyName";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboContractor.DisplayMember = "CompanyName";
            cboContractor.ValueMember = "ContractorID";
            cboContractor.DataSource = dt;
        }
        
        private void LoadRentals()
        {
            string query = @"SELECT r.AgreementID, e.Model AS Equipment, c.CompanyName AS Contractor,
                                    r.StartDate, r.EndDate, r.ReturnStatus
                             FROM RENTALAGREEMENT r
                             JOIN EQUIPMENT e ON e.EquipmentID = r.EquipmentID
                             JOIN CONTRACTOR c ON c.ContractorID = r.ContractorID
                             ORDER BY r.StartDate DESC";
            dgvRentals.DataSource = DatabaseHelper.ExecuteQuery(query);
        }
        
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (cboEquipment.SelectedValue == null || cboContractor.SelectedValue == null)
            {
                ShowMessage("Please select equipment and contractor.", Color.Red);
                return;
            }
            
            string query = @"INSERT INTO RENTALAGREEMENT (EquipmentID, ContractorID, StartDate, EndDate, ReturnStatus) 
                             VALUES (@eq, @cont, @start, @end, 'Active')";
            var parameters = new[]
            {
                new SqlParameter("@eq", cboEquipment.SelectedValue),
                new SqlParameter("@cont", cboContractor.SelectedValue),
                new SqlParameter("@start", dtpStart.Value),
                new SqlParameter("@end", dtpEnd.Value)
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Rental agreement created!", Color.LightGreen);
                LoadRentals();
                
                // Update equipment status
                string updateEq = "UPDATE EQUIPMENT SET Status = 'Rented' WHERE EquipmentID = @id";
                DatabaseHelper.ExecuteNonQuery(updateEq, new[] { new SqlParameter("@id", cboEquipment.SelectedValue) });
                LoadEquipment();
            }
        }
        
        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgvRentals.SelectedRows.Count == 0)
            {
                ShowMessage("Select an agreement to mark as returned.", Color.Red);
                return;
            }
            
            int id = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["AgreementID"].Value);
            string query = @"UPDATE RENTALAGREEMENT SET EndDate = @end, ReturnStatus = 'Returned' 
                             WHERE AgreementID = @id";
            var parameters = new[]
            {
                new SqlParameter("@end", DateTime.Now),
                new SqlParameter("@id", id)
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Agreement marked as returned!", Color.LightGreen);
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
                int id = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["AgreementID"].Value);
                string query = "DELETE FROM RENTALAGREEMENT WHERE AgreementID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", id) });
                
                if (result > 0)
                {
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