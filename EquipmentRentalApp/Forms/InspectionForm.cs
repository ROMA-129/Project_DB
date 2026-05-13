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
        private ComboBox cboEquipment, cboTechnician, cboResult;
        private DateTimePicker dtpDate;
        private TextBox txtNotes;
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
            this.Size = new Size(1100, 650);
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
                Size = new Size(1050, 180),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            Label lblEquipment = new Label { Text = "Equipment:", Location = new Point(20, 25), Size = new Size(100, 25), ForeColor = Color.White };
            cboEquipment = new ComboBox { Location = new Point(130, 23), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblTechnician = new Label { Text = "Technician:", Location = new Point(20, 65), Size = new Size(100, 25), ForeColor = Color.White };
            cboTechnician = new ComboBox { Location = new Point(130, 63), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblDate = new Label { Text = "Inspection Date:", Location = new Point(450, 25), Size = new Size(100, 25), ForeColor = Color.White };
            dtpDate = new DateTimePicker { Location = new Point(570, 23), Size = new Size(180, 25), Format = DateTimePickerFormat.Short };
            
            Label lblResult = new Label { Text = "Result:", Location = new Point(450, 65), Size = new Size(100, 25), ForeColor = Color.White };
            cboResult = new ComboBox { Location = new Point(570, 63), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboResult.Items.AddRange(new object[] { "Pass", "Fail", "Pending" });
            cboResult.SelectedIndex = 0;
            
            Label lblNotes = new Label { Text = "Notes:", Location = new Point(20, 105), Size = new Size(100, 25), ForeColor = Color.White };
            txtNotes = new TextBox { Location = new Point(130, 103), Size = new Size(400, 60), Multiline = true };
            
            btnInsert = new Button { Text = "➕ Log Inspection", Location = new Point(750, 40), Size = new Size(160, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(750, 90), Size = new Size(160, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblEquipment, cboEquipment, lblTechnician, cboTechnician,
                lblDate, dtpDate, lblResult, cboResult, lblNotes, txtNotes,
                btnInsert, btnDelete
            });
            this.Controls.Add(inputPanel);
            
            // Message
            lblMsg = new Label { Location = new Point(20, 260), Size = new Size(500, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvInspections = new DataGridView
            {
                Location = new Point(20, 290),
                Size = new Size(1050, 320),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            this.Controls.Add(dgvInspections);
        }
        
        private void LoadEquipment()
        {
            string query = "SELECT EquipmentID, Model FROM EQUIPMENT ORDER BY Model";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboEquipment.DisplayMember = "Model";
            cboEquipment.ValueMember = "EquipmentID";
            cboEquipment.DataSource = dt;
        }
        
        private void LoadTechnicians()
        {
            string query = "SELECT TechnicianID, FullName FROM TECHNICIAN ORDER BY FullName";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboTechnician.DisplayMember = "FullName";
            cboTechnician.ValueMember = "TechnicianID";
            cboTechnician.DataSource = dt;
        }
        
        private void LoadInspections()
        {
            string query = @"SELECT i.InspectionID, e.Model AS Equipment, t.FullName AS Technician,
                                    i.InspectionDate, i.Result, i.Notes
                             FROM SAFETYINSPECTION i
                             JOIN EQUIPMENT e ON e.EquipmentID = i.EquipmentID
                             JOIN TECHNICIAN t ON t.TechnicianID = i.TechnicianID
                             ORDER BY i.InspectionDate DESC";
            dgvInspections.DataSource = DatabaseHelper.ExecuteQuery(query);
        }
        
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            if (cboEquipment.SelectedValue == null)
            {
                ShowMessage("Please select equipment.", Color.Red);
                return;
            }
            
            string query = @"INSERT INTO SAFETYINSPECTION (EquipmentID, TechnicianID, InspectionDate, Result, Notes) 
                             VALUES (@eq, @tech, @date, @result, @notes)";
            var parameters = new[]
            {
                new SqlParameter("@eq", cboEquipment.SelectedValue),
                new SqlParameter("@tech", cboTechnician.SelectedValue),
                new SqlParameter("@date", dtpDate.Value),
                new SqlParameter("@result", cboResult.SelectedItem?.ToString()),
                new SqlParameter("@notes", txtNotes.Text.Trim())
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Inspection logged successfully!", Color.LightGreen);
                LoadInspections();
                txtNotes.Clear();
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
                string query = "DELETE FROM SAFETYINSPECTION WHERE InspectionID = @id";
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