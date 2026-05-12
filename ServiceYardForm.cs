using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class ServiceYardForm : Form
    {
        static readonly Color BgDark      = Color.FromArgb(15, 23, 42);
        static readonly Color BgCard      = Color.FromArgb(30, 41, 59);
        static readonly Color AccentBlue  = Color.FromArgb(59, 130, 246);
        static readonly Color TextMuted   = Color.FromArgb(148, 163, 184);
        static readonly Color DangerRed   = Color.FromArgb(239, 68, 68);
        static readonly Color AccentGreen = Color.FromArgb(34, 197, 94);

        private DataGridView grid;
        private TextBox txtLocation, txtCapacity, txtContact;
        private Label lblMsg;

        public ServiceYardForm() { InitializeUI(); LoadYards(); }

        private void InitializeUI()
        {
            this.BackColor = BgDark;
            this.Padding   = new Padding(20);

            this.Controls.Add(MainForm.MakeLabel("Service Yard Management", 0, 0, 18, true, AccentBlue));
            this.Controls.Add(MainForm.MakeLabel("Add, update, or remove service yard locations", 0, 34, 10, false, TextMuted));

            var formPanel = new Panel
            {
                Bounds    = new Rectangle(0, 70, 700, 145),
                BackColor = BgCard
            };
            this.Controls.Add(formPanel);

            int lx = 16, fx = 140, r1 = 16, r2 = 64;

            formPanel.Controls.Add(MainForm.MakeLabel("Location:", lx, r1 + 6));
            txtLocation = MainForm.MakeTextBox(fx, r1, 320);
            formPanel.Controls.Add(txtLocation);

            formPanel.Controls.Add(MainForm.MakeLabel("Capacity:", lx + 490, r1 + 6));
            txtCapacity = MainForm.MakeTextBox(fx + 490, r1, 120);
            formPanel.Controls.Add(txtCapacity);

            formPanel.Controls.Add(MainForm.MakeLabel("Contact No:", lx, r2 + 6));
            txtContact = MainForm.MakeTextBox(fx, r2, 250);
            formPanel.Controls.Add(txtContact);

            lblMsg = new Label
            {
                AutoSize  = true,
                Location  = new Point(lx, r2 + 44),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = AccentGreen
            };
            formPanel.Controls.Add(lblMsg);

            var btnAdd = MainForm.MakeButton("➕ Add Yard", 380, r2, AccentGreen, 130);
            btnAdd.Click += BtnInsert_Click;
            formPanel.Controls.Add(btnAdd);

            var btnUpdate = MainForm.MakeButton("✏ Update", 520, r2, AccentBlue, 100);
            btnUpdate.Click += BtnUpdate_Click;
            formPanel.Controls.Add(btnUpdate);

            var btnDelete = MainForm.MakeButton("🗑 Delete", 628, r2, DangerRed, 90);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            grid = MainForm.CreateStyledGrid();
            grid.Bounds = new Rectangle(0, 226, this.Width - 50, this.Height - 240);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grid.SelectionChanged += Grid_SelectionChanged;
            this.Controls.Add(grid);
        }

        private void LoadYards()
        {
            var sql =
                @"SELECT sy.YardID   AS [ID],
                         sy.Location,
                         sy.Capacity,
                         sy.ContactNo AS [Contact],
                         COUNT(e.EquipmentID) AS [Equipment Count]
                  FROM   ServiceYard sy
                  LEFT JOIN Equipment e ON e.YardID = sy.YardID
                  GROUP BY sy.YardID, sy.Location, sy.Capacity, sy.ContactNo
                  ORDER BY sy.Location";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void Grid_SelectionChanged(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            txtLocation.Text = row.Cells["Location"].Value?.ToString();
            txtCapacity.Text = row.Cells["Capacity"].Value?.ToString();
            txtContact.Text  = row.Cells["Contact"].Value?.ToString();
            lblMsg.Text = "";
        }

        // ── F11 – INSERT ServiceYard ────────────────────────────
        private void BtnInsert_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            { ShowMsg("Location is required.", DangerRed); return; }
            if (!int.TryParse(txtCapacity.Text, out int cap))
            { ShowMsg("Capacity must be a whole number.", DangerRed); return; }

            var sql = @"INSERT INTO ServiceYard (Location, Capacity, ContactNo)
                        VALUES (@loc, @cap, @con)";
            var p = new[]
            {
                new SqlParameter("@loc", txtLocation.Text.Trim()),
                new SqlParameter("@cap", cap),
                new SqlParameter("@con", txtContact.Text.Trim())
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Yard added!", AccentGreen); LoadYards(); ClearForm(); }
        }

        // ── F12 – UPDATE ServiceYard ────────────────────────────
        private void BtnUpdate_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to update.", DangerRed); return; }
            int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);
            if (!int.TryParse(txtCapacity.Text, out int cap))
            { ShowMsg("Capacity must be a whole number.", DangerRed); return; }

            var sql = @"UPDATE ServiceYard SET
                            Location  = @loc,
                            Capacity  = @cap,
                            ContactNo = @con
                        WHERE YardID = @id";
            var p = new[]
            {
                new SqlParameter("@loc", txtLocation.Text.Trim()),
                new SqlParameter("@cap", cap),
                new SqlParameter("@con", txtContact.Text.Trim()),
                new SqlParameter("@id",  id)
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Yard updated!", AccentGreen); LoadYards(); }
        }

        private void BtnDelete_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to delete.", DangerRed); return; }
            int id  = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);

            var confirm = MessageBox.Show("Delete this service yard?\nEquipment in this yard will be unassigned.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            // Unassign equipment first, then delete yard
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Equipment SET YardID = NULL WHERE YardID = @id",
                new[] { new SqlParameter("@id", id) });

            int rows = DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM ServiceYard WHERE YardID = @id",
                new[] { new SqlParameter("@id", id) });
            if (rows > 0) { ShowMsg("Yard deleted.", Color.FromArgb(234, 179, 8)); LoadYards(); ClearForm(); }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ServiceYardForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "ServiceYardForm";
            this.Load += new System.EventHandler(this.ServiceYardForm_Load);
            this.ResumeLayout(false);

        }

        private void ServiceYardForm_Load(object sender, EventArgs e)
        {

        }

        private void ClearForm() { txtLocation.Text = txtCapacity.Text = txtContact.Text = ""; }
        private void ShowMsg(string msg, Color color) { lblMsg.Text = msg; lblMsg.ForeColor = color; }
    }
}
