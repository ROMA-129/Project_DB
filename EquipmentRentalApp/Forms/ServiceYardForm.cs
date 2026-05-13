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
        private TextBox txtYardName, txtLocation, txtCapacity;
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
                Size = new Size(950, 155),
                BackColor = Color.FromArgb(60, 60, 65)
            };

            // FIX: added YardName field (required by DB), removed non-existent ContactNo field
            Label lblYardName = new Label { Text = "Yard Name:", Location = new Point(20, 20), Size = new Size(100, 25), ForeColor = Color.White };
            txtYardName = new TextBox { Location = new Point(130, 18), Size = new Size(300, 25) };

            Label lblLocation = new Label { Text = "Location:", Location = new Point(20, 60), Size = new Size(100, 25), ForeColor = Color.White };
            txtLocation = new TextBox { Location = new Point(130, 58), Size = new Size(300, 25) };
            
            Label lblCapacity = new Label { Text = "Capacity:", Location = new Point(20, 100), Size = new Size(100, 25), ForeColor = Color.White };
            txtCapacity = new TextBox { Location = new Point(130, 98), Size = new Size(150, 25) };
            
            btnInsert = new Button { Text = "➕ Add Yard", Location = new Point(500, 20), Size = new Size(120, 35), BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnInsert.Click += BtnInsert_Click;
            
            btnUpdate = new Button { Text = "✏ Update", Location = new Point(630, 20), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 123, 255), ForeColor = Color.White };
            btnUpdate.Click += BtnUpdate_Click;
            
            btnDelete = new Button { Text = "🗑 Delete", Location = new Point(740, 20), Size = new Size(100, 35), BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            
            btnClear = new Button { Text = "Clear", Location = new Point(850, 20), Size = new Size(80, 35), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };
            btnClear.Click += (s, e) => ClearForm();
            
            inputPanel.Controls.AddRange(new Control[] { 
                lblYardName, txtYardName, lblLocation, txtLocation, lblCapacity, txtCapacity,
                btnInsert, btnUpdate, btnDelete, btnClear
            });
            this.Controls.Add(inputPanel);
            
            // Message
            lblMsg = new Label { Location = new Point(20, 235), Size = new Size(400, 25), ForeColor = Color.LightGreen };
            this.Controls.Add(lblMsg);
            
            // DataGridView
            dgvYards = new DataGridView
            {
                Location = new Point(20, 265),
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
            // FIX: correct table name "ServiceYard", correct columns (YardName, Location, Capacity — no ContactNo)
            string query = "SELECT YardID, YardName, Location, Capacity FROM ServiceYard ORDER BY YardName";
            dgvYards.DataSource = DatabaseHelper.ExecuteQuery(query);
            if (dgvYards.Columns["YardID"] != null)
                dgvYards.Columns["YardID"].Visible = false;
        }
        
        private void DgvYards_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvYards.SelectedRows.Count > 0)
            {
                var row = dgvYards.SelectedRows[0];
                txtYardName.Text = row.Cells["YardName"].Value?.ToString();
                txtLocation.Text = row.Cells["Location"].Value?.ToString();
                txtCapacity.Text = row.Cells["Capacity"].Value?.ToString();
                lblMsg.Text = "";
            }
        }
        
        private void BtnInsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtYardName.Text))
            {
                ShowMessage("Yard Name is required!", Color.Red);
                return;
            }
            
            if (!int.TryParse(txtCapacity.Text, out int capacity))
                capacity = 0;
            
            // FIX: correct table name "ServiceYard", correct columns (no ContactNo)
            string query = @"INSERT INTO ServiceYard (YardName, Location, Capacity) 
                             VALUES (@yardName, @location, @capacity)";
            
            var parameters = new[]
            {
                new SqlParameter("@yardName",  txtYardName.Text.Trim()),
                new SqlParameter("@location",  txtLocation.Text.Trim()),
                new SqlParameter("@capacity",  capacity)
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
            
            if (string.IsNullOrWhiteSpace(txtYardName.Text))
            {
                ShowMessage("Yard Name is required!", Color.Red);
                return;
            }
            
            int id = Convert.ToInt32(dgvYards.SelectedRows[0].Cells["YardID"].Value);
            
            // FIX: correct table name and columns
            string query = @"UPDATE ServiceYard 
                             SET YardName  = @yardName, 
                                 Location  = @location, 
                                 Capacity  = @capacity
                             WHERE YardID = @id";
            
            var parameters = new[]
            {
                new SqlParameter("@yardName",  txtYardName.Text.Trim()),
                new SqlParameter("@location",  txtLocation.Text.Trim()),
                new SqlParameter("@capacity",  int.TryParse(txtCapacity.Text, out int c) ? c : 0),
                new SqlParameter("@id",        id)
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
            
            string yardName = dgvYards.SelectedRows[0].Cells["YardName"].Value?.ToString();
            
            var confirm = MessageBox.Show(
                $"Delete service yard:\n\"{yardName}\"?\n\nEquipment in this yard will be unassigned!",
                "Confirm Delete", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);
                
            if (confirm == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvYards.SelectedRows[0].Cells["YardID"].Value);
                
                // First unassign equipment from this yard
                string unassignEq = "UPDATE Equipment SET YardID = NULL WHERE YardID = @id";
                DatabaseHelper.ExecuteNonQuery(unassignEq, new[] { new SqlParameter("@id", id) });
                
                // FIX: correct table name "ServiceYard"
                string query = "DELETE FROM ServiceYard WHERE YardID = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new[] { new SqlParameter("@id", id) });
                
                if (result > 0)
                {
                    ShowMessage("✓ Service yard deleted.", Color.Yellow);
                    LoadYards();
                    ClearForm();
                }
            }
        }
        
        private void ClearForm()
        {
            txtYardName.Clear();
            txtLocation.Clear();
            txtCapacity.Clear();
            lblMsg.Text = "";
        }
        
        private void ShowMessage(string msg, Color color)
        {
            lblMsg.Text = msg;
            lblMsg.ForeColor = color;
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