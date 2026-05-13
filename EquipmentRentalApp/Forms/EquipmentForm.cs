using System;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using EquipmentRentalApp.Database;

namespace EquipmentRentalApp.Forms
{
    public partial class EquipmentForm : Form
    {
        private DataGridView dgvEquipment;
        private TextBox txtModel, txtEnginePower, txtHourlyRate, txtSearch;
        private ComboBox cboStatus, cboYard;
        private Button btnInsert, btnUpdate, btnDelete, btnSearch, btnClear;
        private Label lblMsg;
        
        public EquipmentForm()
        {
            InitializeUI();
            LoadEquipment();
            LoadYards();
        }
        
        private void InitializeUI()
        {
            this.Text = "Equipment Management";
            this.Size = new Size(1100, 650);
            this.BackColor = Color.FromArgb(45, 45, 50);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Title
            Label lblTitle = new Label
            {
                Text = "🔧 Equipment Management",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(400, 40)
            };
            this.Controls.Add(lblTitle);
            
            // Search Panel
            Panel searchPanel = new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(1050, 50),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            Label lblSearch = new Label { Text = "Search:", Location = new Point(15, 15), Size = new Size(60, 25), ForeColor = Color.White };
            txtSearch = new TextBox { Location = new Point(80, 13), Size = new Size(200, 25) };
            btnSearch = new Button { Text = "🔍 Search", Location = new Point(290, 10), Size = new Size(100, 30), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnSearch.Click += (s, e) => SearchEquipment();
            
            Button btnShowAll = new Button { Text = "Show All", Location = new Point(400, 10), Size = new Size(100, 30), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };
            btnShowAll.Click += (s, e) => LoadEquipment();
            
            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnShowAll });
            this.Controls.Add(searchPanel);
            
            // Input Panel
            Panel inputPanel = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(1050, 160),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            // Row 1
            Label lblModel = new Label { Text = "Model:", Location = new Point(20, 20), Size = new Size(100, 25), ForeColor = Color.White };
            txtModel = new TextBox { Location = new Point(130, 18), Size = new Size(200, 25) };
            
            Label lblEnginePower = new Label { Text = "Engine Power:", Location = new Point(360, 20), Size = new Size(110, 25), ForeColor = Color.White };
            txtEnginePower = new TextBox { Location = new Point(480, 18), Size = new Size(150, 25) };
            
            Label lblHourlyRate = new Label { Text = "Hourly Rate:", Location = new Point(660, 20), Size = new Size(100, 25), ForeColor = Color.White };
            txtHourlyRate = new TextBox { Location = new Point(770, 18), Size = new Size(120, 25) };
            
            // Row 2
            Label lblStatus = new Label { Text = "Status:", Location = new Point(20, 60), Size = new Size(100, 25), ForeColor = Color.White };
            cboStatus = new ComboBox { Location = new Point(130, 58), Size = new Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new object[] { "Available", "Rented", "Under Maintenance" });
            cboStatus.SelectedIndex = 0;
            
            Label lblYard = new Label { Text = "Service Yard:", Location = new Point(360, 60), Size = new Size(110, 25), ForeColor = Color.White };
            cboYard = new ComboBox { Location = new Point(480, 58), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            // Buttons
            btnInsert = new Button { Text = "➕ Add Equipment", Location = new Point(550, 105), Size = new Size(130, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnUpdate = new Button { Text = "✏ Update", Location = new Point(695, 105), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnUpdate.Click += BtnUpdate_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(805, 105), Size = new Size(100, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            btnClear = new Button { Text = "Clear", Location = new Point(915, 105), Size = new Size(100, 35), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };
            btnClear.Click += (s, e) => ClearForm();
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblModel, txtModel, lblEnginePower, txtEnginePower, lblHourlyRate, txtHourlyRate,
                lblStatus, cboStatus, lblYard, cboYard,
                btnInsert, btnUpdate, btnDelete, btnClear
            });
            this.Controls.Add(inputPanel);
            
            // Message Label
            lblMsg = new Label { Location = new Point(20, 300), Size = new Size(400, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvEquipment = new DataGridView
            {
                Location = new Point(20, 330),
                Size = new Size(1050, 280),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvEquipment.SelectionChanged += DgvEquipment_SelectionChanged;
            this.Controls.Add(dgvEquipment);
        }
        
        private void LoadYards()
        {
            // FIX: correct table name "ServiceYard", correct column "YardName"
            string query = "SELECT YardID, YardName FROM ServiceYard ORDER BY YardName";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboYard.Items.Clear();
            cboYard.Items.Add(new { YardID = (int?)null, YardName = "-- None --" });
            foreach (DataRow row in dt.Rows)
            {
                cboYard.Items.Add(new { YardID = (int?)Convert.ToInt32(row["YardID"]), YardName = row["YardName"].ToString() });
            }
            cboYard.DisplayMember = "YardName";
            cboYard.SelectedIndex = 0;
        }
        
        private void LoadEquipment()
        {
            // FIX: correct table names "Equipment", "ServiceYard"; removed non-existent Location column
            string query = @"SELECT e.EquipmentID, e.Model, e.EnginePower, e.HourlyRate,
                                    e.Status, ISNULL(s.YardName, '—') AS Yard
                             FROM Equipment e
                             LEFT JOIN ServiceYard s ON s.YardID = e.YardID
                             ORDER BY e.EquipmentID";
            dgvEquipment.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgvEquipment.Columns["EquipmentID"] != null)
                dgvEquipment.Columns["EquipmentID"].Visible = false;
        }
        
        private void SearchEquipment()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadEquipment();
                return;
            }
            
            string query = @"SELECT e.EquipmentID, e.Model, e.EnginePower, e.HourlyRate,
                                    e.Status, ISNULL(s.YardName, '—') AS Yard
                             FROM Equipment e
                             LEFT JOIN ServiceYard s ON s.YardID = e.YardID
                             WHERE e.Model LIKE @search
                             ORDER BY e.EquipmentID";
            var param = new[] { new SqlParameter("@search", "%" + txtSearch.Text.Trim() + "%") };
            dgvEquipment.DataSource = DatabaseHelper.ExecuteQuery(query, param);
        }
        
        private void DgvEquipment_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count > 0)
            {
                var row = dgvEquipment.SelectedRows[0];
                txtModel.Text = row.Cells["Model"].Value?.ToString();
                txtEnginePower.Text = row.Cells["EnginePower"].Value?.ToString();
                txtHourlyRate.Text = row.Cells["HourlyRate"].Value?.ToString();
                string status = row.Cells["Status"].Value?.ToString();
                if (!string.IsNullOrEmpty(status))
                    cboStatus.SelectedItem = status;
            }
        }
        
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                ShowMessage("Model is required!", Color.Red);
                return;
            }
            
            if (!decimal.TryParse(txtEnginePower.Text, out decimal engine))
                engine = 0;
            if (!decimal.TryParse(txtHourlyRate.Text, out decimal rate))
                rate = 0;

            // FIX: correct column name "EnginePower", removed non-existent "Location" column
            string query = @"INSERT INTO Equipment (Model, EnginePower, HourlyRate, Status, YardID) 
                             VALUES (@model, @engine, @rate, @status, NULL)";
            var parameters = new[]
            {
                new SqlParameter("@model", txtModel.Text.Trim()),
                new SqlParameter("@engine", engine),
                new SqlParameter("@rate", rate),
                new SqlParameter("@status", cboStatus.SelectedItem?.ToString() ?? "Available")
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Equipment added successfully!", Color.LightGreen);
                LoadEquipment();
                ClearForm();
            }
        }
        
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count == 0)
            {
                ShowMessage("Select a row to update.", Color.Red);
                return;
            }
            
            int id = Convert.ToInt32(dgvEquipment.SelectedRows[0].Cells["EquipmentID"].Value);

            if (!decimal.TryParse(txtEnginePower.Text, out decimal engine))
                engine = 0;

            // FIX: correct column name "EnginePower", removed non-existent "Location" column
            string query = @"UPDATE Equipment SET Model = @model, EnginePower = @engine, 
                             HourlyRate = @rate, Status = @status 
                             WHERE EquipmentID = @id";
            var parameters = new[]
            {
                new SqlParameter("@model", txtModel.Text.Trim()),
                new SqlParameter("@engine", engine),
                new SqlParameter("@rate", decimal.TryParse(txtHourlyRate.Text, out decimal r) ? r : 0),
                new SqlParameter("@status", cboStatus.SelectedItem?.ToString() ?? "Available"),
                new SqlParameter("@id", id)
            };
            
            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
            if (result > 0)
            {
                ShowMessage("Equipment updated successfully!", Color.LightGreen);
                LoadEquipment();
            }
        }
        
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count == 0)
            {
                ShowMessage("Select a row to delete.", Color.Red);
                return;
            }
            
            var confirm = MessageBox.Show("Delete this equipment?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvEquipment.SelectedRows[0].Cells["EquipmentID"].Value);
                string query = "DELETE FROM Equipment WHERE EquipmentID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", id) });
                
                if (result > 0)
                {
                    ShowMessage("Equipment deleted!", Color.Yellow);
                    LoadEquipment();
                    ClearForm();
                }
            }
        }
        
        private void ClearForm()
        {
            txtModel.Clear();
            txtEnginePower.Clear();
            txtHourlyRate.Clear();
            cboStatus.SelectedIndex = 0;
            cboYard.SelectedIndex = 0;
        }
        
        private void ShowMessage(string msg, Color color)
        {
            lblMsg.Text = msg;
            lblMsg.ForeColor = color;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) => { lblMsg.Text = ""; timer.Stop(); };
            timer.Start();
        }
    }
}