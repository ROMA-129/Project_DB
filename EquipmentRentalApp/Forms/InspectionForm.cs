using System;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using EquipmentRentalApp.Database;

namespace EquipmentRentalApp.Forms
{
    public partial class InspectionForm : Form
    {
        private DataGridView dgvInspections;
        private ComboBox cboEquipment, cboTechnician, cboStatus;
        private DateTimePicker dtpDate;
        private TextBox txtChecklist, txtMaintenance;
        private Button btnInsert, btnDelete;
        private Label lblMsg;
        
        public InspectionForm()
        {
            InitializeUI();
            LoadEquipment();
            LoadTechnicians();
            LoadInspections();
        }
        
        private void InitializeUI()
        {
            this.Text = "Safety Inspection Management";
            this.Size = new Size(1100, 700);
            this.BackColor = Color.FromArgb(45, 45, 50);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            Label lblTitle = new Label
            {
                Text = "🔍 Safety Inspections",
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
                Size = new Size(1050, 220),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            Label lblEquipment = new Label { Text = "Equipment:", Location = new Point(20, 25), Size = new Size(110, 25), ForeColor = Color.White };
            cboEquipment = new ComboBox { Location = new Point(140, 23), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblTechnician = new Label { Text = "Technician:", Location = new Point(20, 65), Size = new Size(110, 25), ForeColor = Color.White };
            cboTechnician = new ComboBox { Location = new Point(140, 63), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblDate = new Label { Text = "Inspection Date:", Location = new Point(430, 25), Size = new Size(120, 25), ForeColor = Color.White };
            dtpDate = new DateTimePicker { Location = new Point(560, 23), Size = new Size(180, 25), Format = DateTimePickerFormat.Short };
            
            // FIX: renamed to InspectionStatus to match DB column
            Label lblStatus = new Label { Text = "Status:", Location = new Point(430, 65), Size = new Size(120, 25), ForeColor = Color.White };
            cboStatus = new ComboBox { Location = new Point(560, 63), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new object[] { "Pass", "Fail", "Pending" });
            cboStatus.SelectedIndex = 0;
            
            // FIX: renamed to ConditionChecklist to match DB column
            Label lblChecklist = new Label { Text = "Condition Checklist:", Location = new Point(20, 105), Size = new Size(120, 25), ForeColor = Color.White };
            txtChecklist = new TextBox { Location = new Point(140, 103), Size = new Size(350, 25) };

            // FIX: added MaintenanceWork field to match DB column
            Label lblMaintenance = new Label { Text = "Maintenance Work:", Location = new Point(20, 145), Size = new Size(120, 25), ForeColor = Color.White };
            txtMaintenance = new TextBox { Location = new Point(140, 143), Size = new Size(350, 25) };
            
            btnInsert = new Button { Text = "➕ Log Inspection", Location = new Point(800, 40), Size = new Size(160, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(800, 90), Size = new Size(160, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblEquipment, cboEquipment, lblTechnician, cboTechnician,
                lblDate, dtpDate, lblStatus, cboStatus,
                lblChecklist, txtChecklist, lblMaintenance, txtMaintenance,
                btnInsert, btnDelete
            });
            this.Controls.Add(inputPanel);
            
            // Message
            lblMsg = new Label { Location = new Point(20, 300), Size = new Size(500, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvInspections = new DataGridView
            {
                Location = new Point(20, 330),
                Size = new Size(1050, 330),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(dgvInspections);
        }
        
        private void LoadEquipment()
        {
            // FIX: correct table name "Equipment"
            string query = "SELECT EquipmentID, Model FROM Equipment ORDER BY Model";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboEquipment.DisplayMember = "Model";
            cboEquipment.ValueMember = "EquipmentID";
            cboEquipment.DataSource = dt;
        }
        
        private void LoadTechnicians()
        {
            // FIX: correct table name "Technician", correct column "TechnicianName" (not "FullName")
            string query = "SELECT TechnicianID, TechnicianName FROM Technician ORDER BY TechnicianName";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboTechnician.DisplayMember = "TechnicianName";
            cboTechnician.ValueMember = "TechnicianID";
            cboTechnician.DataSource = dt;
        }
        
        private void LoadInspections()
        {
            // FIX: correct table names and column names
            // "SafetyInspection", "Technician"; column "TechnicianName", "InspectionStatus", "ConditionChecklist", "MaintenanceWork"
            string query = @"SELECT i.InspectionID, e.Model AS Equipment, t.TechnicianName AS Technician,
                                    i.InspectionDate, i.InspectionStatus AS Status,
                                    i.ConditionChecklist AS Checklist, i.MaintenanceWork
                             FROM SafetyInspection i
                             JOIN Equipment e ON e.EquipmentID = i.EquipmentID
                             JOIN Technician t ON t.TechnicianID = i.TechnicianID
                             ORDER BY i.InspectionDate DESC";
            dgvInspections.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgvInspections.Columns["InspectionID"] != null)
                dgvInspections.Columns["InspectionID"].Visible = false;
        }
        
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            if (cboEquipment.SelectedValue == null)
            {
                ShowMessage("Please select equipment.", Color.Red);
                return;
            }
            if (cboTechnician.SelectedValue == null)
            {
                ShowMessage("Please select a technician.", Color.Red);
                return;
            }
            
            // FIX: correct table name and column names matching actual DB schema
            string query = @"INSERT INTO SafetyInspection (EquipmentID, TechnicianID, InspectionDate, ConditionChecklist, MaintenanceWork, InspectionStatus) 
                             VALUES (@eq, @tech, @date, @checklist, @maintenance, @status)";
            var parameters = new[]
            {
                new SqlParameter("@eq",          cboEquipment.SelectedValue),
                new SqlParameter("@tech",         cboTechnician.SelectedValue),
                new SqlParameter("@date",         dtpDate.Value.Date),
                new SqlParameter("@checklist",    txtChecklist.Text.Trim()),
                new SqlParameter("@maintenance",  txtMaintenance.Text.Trim()),
                new SqlParameter("@status",       cboStatus.SelectedItem?.ToString())
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Inspection logged successfully!", Color.LightGreen);
                LoadInspections();
                txtChecklist.Clear();
                txtMaintenance.Clear();
            }
        }
        
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInspections.SelectedRows.Count == 0)
            {
                ShowMessage("Select an inspection to delete.", Color.Red);
                return;
            }
            
            var confirm = MessageBox.Show("Delete this inspection record?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvInspections.SelectedRows[0].Cells["InspectionID"].Value);
                // FIX: correct table name "SafetyInspection"
                string query = "DELETE FROM SafetyInspection WHERE InspectionID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", id) });
                
                if (result > 0)
                {
                    ShowMessage("Inspection deleted.", Color.Yellow);
                    LoadInspections();
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