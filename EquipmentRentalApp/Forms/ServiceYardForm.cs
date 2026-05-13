using System;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using EquipmentRentalApp.Database;

namespace EquipmentRentalApp.Forms
{
    public partial class ServiceYardForm : Form
    {
        private DataGridView dgvYards;
        private TextBox txtLocation, txtCapacity, txtContact;
        private Button btnInsert, btnUpdate, btnDelete, btnClear;
        private Label lblMsg;
        
        public ServiceYardForm()
        {
            InitializeUI();
            LoadYards();
        }
        
        private void InitializeUI()
        {
            this.Text = "Service Yard Management";
            this.Size = new Size(1000, 600);
            this.BackColor = Color.FromArgb(45, 45, 50);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            Label lblTitle = new Label
            {
                Text = "🏭 Service Yards",
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
                Size = new Size(950, 150),
                BackColor = Color.FromArgb(60, 60, 65)
            };
            
            Label lblLocation = new Label { Text = "Location:", Location = new Point(20, 25), Size = new Size(100, 25), ForeColor = Color.White };
            txtLocation = new TextBox { Location = new Point(130, 23), Size = new Size(300, 25) };
            
            Label lblCapacity = new Label { Text = "Capacity:", Location = new Point(20, 65), Size = new Size(100, 25), ForeColor = Color.White };
            txtCapacity = new TextBox { Location = new Point(130, 63), Size = new Size(150, 25) };
            
            Label lblContact = new Label { Text = "Contact No:", Location = new Point(20, 105), Size = new Size(100, 25), ForeColor = Color.White };
            txtContact = new TextBox { Location = new Point(130, 103), Size = new Size(200, 25) };
            
            btnInsert = new Button { Text = "➕ Add Yard", Location = new Point(500, 25), Size = new Size(120, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnUpdate = new Button { Text = "✏ Update", Location = new Point(630, 25), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnUpdate.Click += BtnUpdate_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(740, 25), Size = new Size(100, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            btnClear = new Button { Text = "Clear", Location = new Point(850, 25), Size = new Size(80, 35), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };
            btnClear.Click += (s, e) => ClearForm();
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblLocation, txtLocation, lblCapacity, txtCapacity, lblContact, txtContact,
                btnInsert, btnUpdate, btnDelete, btnClear
            });
            this.Controls.Add(inputPanel);
            
            // Message
            lblMsg = new Label { Location = new Point(20, 230), Size = new Size(400, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvYards = new DataGridView
            {
                Location = new Point(20, 260),
                Size = new Size(950, 300),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvYards.SelectionChanged += DgvYards_SelectionChanged;
            this.Controls.Add(dgvYards);
        }
        
        private void LoadYards()
        {
            string query = "SELECT YardID, Location, Capacity, ContactNo FROM SERVICEYARD ORDER BY Location";
            dgvYards.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgvYards.Columns["YardID"] != null)
                dgvYards.Columns["YardID"].Visible = false;
                
            // Set column headers
            if (dgvYards.Columns["Location"] != null)
                dgvYards.Columns["Location"].HeaderText = "Location";
            if (dgvYards.Columns["Capacity"] != null)
                dgvYards.Columns["Capacity"].HeaderText = "Capacity";
            if (dgvYards.Columns["ContactNo"] != null)
                dgvYards.Columns["ContactNo"].HeaderText = "Contact No";
        }
        
        private void DgvYards_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvYards.SelectedRows.Count > 0)
            {
                var row = dgvYards.SelectedRows[0];
                txtLocation.Text = row.Cells["Location"].Value?.ToString();
                txtCapacity.Text = row.Cells["Capacity"].Value?.ToString();
                txtContact.Text = row.Cells["ContactNo"].Value?.ToString();
                lblMsg.Text = "";
            }
        }
        
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                ShowMessage("Location is required!", Color.Red);
                return;
            }
            
            if (!int.TryParse(txtCapacity.Text, out int capacity))
                capacity = 0;
            
            string query = @"INSERT INTO SERVICEYARD (Location, Capacity, ContactNo) 
                             VALUES (@location, @capacity, @contact)";
            
            var parameters = new[]
            {
                new SqlParameter("@location", txtLocation.Text.Trim()),
                new SqlParameter("@capacity", capacity),
                new SqlParameter("@contact", txtContact.Text.Trim())
            };
            
            try
            {
                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    ShowMessage("✓ Service yard added successfully!", Color.LightGreen);
                    LoadYards();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, Color.Red);
            }
        }
        
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvYards.SelectedRows.Count == 0)
            {
                ShowMessage("Please select a yard to update.", Color.Red);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                ShowMessage("Location is required!", Color.Red);
                return;
            }
            
            int id = Convert.ToInt32(dgvYards.SelectedRows[0].Cells["YardID"].Value);
            
            string query = @"UPDATE SERVICEYARD 
                             SET Location = @location, 
                                 Capacity = @capacity, 
                                 ContactNo = @contact 
                             WHERE YardID = @id";
            
            var parameters = new[]
            {
                new SqlParameter("@location", txtLocation.Text.Trim()),
                new SqlParameter("@capacity", int.TryParse(txtCapacity.Text, out int c) ? c : 0),
                new SqlParameter("@contact", txtContact.Text.Trim()),
                new SqlParameter("@id", id)
            };
            
            try
            {
                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    ShowMessage("✓ Service yard updated successfully!", Color.LightGreen);
                    LoadYards();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, Color.Red);
            }
        }
        
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvYards.SelectedRows.Count == 0)
            {
                ShowMessage("Please select a yard to delete.", Color.Red);
                return;
            }
            
            string location = dgvYards.SelectedRows[0].Cells["Location"].Value?.ToString();
            
            var confirm = MessageBox.Show(
                $"Delete service yard:\n\"{location}\"?\n\nEquipment in this yard will be unassigned!",
                "Confirm Delete", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);
                
            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvYards.SelectedRows[0].Cells["YardID"].Value);
                
                // First unassign equipment from this yard
                string unassignEq = "UPDATE EQUIPMENT SET YardID = NULL WHERE YardID = @id";
                DatabaseHelper.ExecuteNonQuery(unassignEq, new[] { new SqlParameter("@id", id) });
                
                // Then delete the yard
                string query = "DELETE FROM SERVICEYARD WHERE YardID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", id) });
                
                if (result > 0)
                {
                    ShowMessage("✓ Service yard deleted successfully!", Color.Yellow);
                    LoadYards();
                    ClearForm();
                }
            }
        }
        
        private void ClearForm()
        {
            txtLocation.Clear();
            txtCapacity.Clear();
            txtContact.Clear();
            lblMsg.Text = "";
        }
        
        private void ShowMessage(string msg, Color color)
        {
            lblMsg.Text = msg;
            lblMsg.ForeColor = color;
            
            // Auto-clear after 3 seconds
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) => { 
                if (lblMsg.Text == msg) 
                    lblMsg.Text = ""; 
                timer.Stop(); 
            };
            timer.Start();
        }
    }
}