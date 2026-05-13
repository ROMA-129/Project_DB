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
        private TextBox txtModel, txtEnginePower, txtHourlyRate, txtLocation, txtSearch;
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
                Size = new Size(1050, 180),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            // Row 1
            Label lblModel = new Label { Text = "Model:", Location = new Point(20, 20), Size = new Size(100, 25), ForeColor = Color.White };
            txtModel = new TextBox { Location = new Point(130, 18), Size = new Size(200, 25) };
            
            Label lblEnginePower = new Label { Text = "Engine Power:", Location = new Point(360, 20), Size = new Size(100, 25), ForeColor = Color.White };
            txtEnginePower = new TextBox { Location = new Point(470, 18), Size = new Size(150, 25) };
            
            Label lblHourlyRate = new Label { Text = "Hourly Rate:", Location = new Point(650, 20), Size = new Size(100, 25), ForeColor = Color.White };
            txtHourlyRate = new TextBox { Location = new Point(760, 18), Size = new Size(120, 25) };
            
            // Row 2
            Label lblStatus = new Label { Text = "Status:", Location = new Point(20, 60), Size = new Size(100, 25), ForeColor = Color.White };
            cboStatus = new ComboBox { Location = new Point(130, 58), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new object[] { "Available", "Rented", "Maintenance" });
            cboStatus.SelectedIndex = 0;
            
            Label lblYard = new Label { Text = "Service Yard:", Location = new Point(360, 60), Size = new Size(100, 25), ForeColor = Color.White };
            cboYard = new ComboBox { Location = new Point(470, 58), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label lblLocation = new Label { Text = "Location:", Location = new Point(20, 100), Size = new Size(100, 25), ForeColor = Color.White };
            txtLocation = new TextBox { Location = new Point(130, 98), Size = new Size(300, 25) };
            
            // Buttons
            btnInsert = new Button { Text = "➕ Add Equipment", Location = new Point(550, 95), Size = new Size(130, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnUpdate = new Button { Text = "✏ Update", Location = new Point(695, 95), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnUpdate.Click += BtnUpdate_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(805, 95), Size = new Size(100, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            btnClear = new Button { Text = "Clear", Location = new Point(915, 95), Size = new Size(100, 35), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };
            btnClear.Click += (s, e) => ClearForm();
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblModel, txtModel, lblEnginePower, txtEnginePower, lblHourlyRate, txtHourlyRate,
                lblStatus, cboStatus, lblYard, cboYard, lblLocation, txtLocation,
                btnInsert, btnUpdate, btnDelete, btnClear
            });
            this.Controls.Add(inputPanel);
            
            // Message Label
            lblMsg = new Label { Location = new Point(20, 320), Size = new Size(400, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvEquipment = new DataGridView
            {
                Location = new Point(20, 350),
                Size = new Size(1050, 260),
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
            string query = "SELECT YardID, Location FROM SERVICEYARD ORDER BY Location";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            cboYard.Items.Clear();
            cboYard.Items.Add("-- None --");
            foreach (DataRow row in dt.Rows)
            {
                cboYard.Items.Add($"{row["Location"]}");
            }
            cboYard.SelectedIndex = 0;
        }
        
        private void LoadEquipment()
        {
            string query = @"SELECT e.EquipmentID, e.Model, e.EnginePower, e.HourlyRate, 
                                    e.Location, e.Status, ISNULL(s.Location, '—') AS Yard
                             FROM EQUIPMENT e
                             LEFT JOIN SERVICEYARD s ON s.YardID = e.YardID
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
                                    e.Location, e.Status, ISNULL(s.Location, '—') AS Yard
                             FROM EQUIPMENT e
                             LEFT JOIN SERVICEYARD s ON s.YardID = e.YardID
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
                txtLocation.Text = row.Cells["Location"].Value?.ToString();
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
            
            if (!decimal.TryParse(txtHourlyRate.Text, out decimal rate))
                rate = 0;
            
            string query = @"INSERT INTO EQUIPMENT (Model, Engineer, HourlyRate, Location, Status, YardID) 
                             VALUES (@model, @engine, @rate, @loc, @status, NULL)";
            var parameters = new[]
            {
                new SqlParameter("@model", txtModel.Text.Trim()),
                new SqlParameter("@engine", txtEnginePower.Text.Trim()),
                new SqlParameter("@rate", rate),
                new SqlParameter("@loc", txtLocation.Text.Trim()),
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
            
            string query = @"UPDATE EQUIPMENT SET Model = @model, Engineer = @engine, 
                             HourlyRate = @rate, Location = @loc, Status = @status 
                             WHERE EquipmentID = @id";
            var parameters = new[]
            {
                new SqlParameter("@model", txtModel.Text.Trim()),
                new SqlParameter("@engine", txtEnginePower.Text.Trim()),
                new SqlParameter("@rate", decimal.TryParse(txtHourlyRate.Text, out decimal r) ? r : 0),
                new SqlParameter("@loc", txtLocation.Text.Trim()),
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
                string query = "DELETE FROM EQUIPMENT WHERE EquipmentID = @id";
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
            txtLocation.Clear();
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