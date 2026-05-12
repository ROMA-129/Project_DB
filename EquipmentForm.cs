using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class EquipmentForm : Form
    {
        static readonly Color BgDark  = Color.FromArgb(15, 23, 42);
        static readonly Color BgCard  = Color.FromArgb(30, 41, 59);
        static readonly Color AccentBlue = Color.FromArgb(59, 130, 246);
        static readonly Color TextLight  = Color.FromArgb(226, 232, 240);
        static readonly Color TextMuted  = Color.FromArgb(148, 163, 184);
        static readonly Color DangerRed  = Color.FromArgb(239, 68, 68);

        private DataGridView grid;
        private TextBox txtModel, txtPower, txtRate, txtLocation, txtSearch;
        private ComboBox cboStatus, cboYard;
        private Label lblMsg;

        public EquipmentForm() { InitializeUI(); LoadEquipment(); }

        private void InitializeUI()
        {
            this.BackColor = BgDark;
            this.Padding   = new Padding(20);

            // Title
            this.Controls.Add(MainForm.MakeLabel("Equipment Management", 0, 0, 18, true, AccentBlue));
            this.Controls.Add(MainForm.MakeLabel("Add, update, or remove equipment records", 0, 34, 10, false, TextMuted));

            // Search
            this.Controls.Add(MainForm.MakeLabel("Search by Model:", 0, 70, 10));
            txtSearch = MainForm.MakeTextBox(130, 66, 240);
            this.Controls.Add(txtSearch);
            var btnSearch = MainForm.MakeButton("🔍 Search", 382, 66, AccentBlue, 110);
            btnSearch.Click += (s, e) => SearchEquipment();
            this.Controls.Add(btnSearch);
            var btnAll = MainForm.MakeButton("Show All", 500, 66, Color.FromArgb(71, 85, 105), 90);
            btnAll.Click += (s, e) => LoadEquipment();
            this.Controls.Add(btnAll);

            // Form panel
            var formPanel = new Panel
            {
                Bounds    = new Rectangle(0, 110, 780, 165),
                BackColor = BgCard,
                Padding   = new Padding(16)
            };
            this.Controls.Add(formPanel);

            int lx = 16, fx = 150, row1 = 16, row2 = 64, row3 = 112;

            formPanel.Controls.Add(MainForm.MakeLabel("Model:", lx, row1 + 6));
            txtModel = MainForm.MakeTextBox(fx, row1, 220);
            formPanel.Controls.Add(txtModel);

            formPanel.Controls.Add(MainForm.MakeLabel("Engine Power:", lx + 390, row1 + 6));
            txtPower = MainForm.MakeTextBox(fx + 390, row1, 180);
            formPanel.Controls.Add(txtPower);

            formPanel.Controls.Add(MainForm.MakeLabel("Hourly Rate:", lx, row2 + 6));
            txtRate = MainForm.MakeTextBox(fx, row2, 140);
            formPanel.Controls.Add(txtRate);

            formPanel.Controls.Add(MainForm.MakeLabel("Status:", lx + 250, row2 + 6));
            cboStatus = MainForm.MakeCombo(fx + 250, row2, 140);
            cboStatus.Items.AddRange(new[] { "Available", "Rented", "Maintenance" });
            cboStatus.SelectedIndex = 0;
            formPanel.Controls.Add(cboStatus);

            formPanel.Controls.Add(MainForm.MakeLabel("Service Yard:", lx + 390 + 10, row2 + 6));
            cboYard = MainForm.MakeCombo(fx + 390, row2, 200);
            formPanel.Controls.Add(cboYard);
            LoadYards();

            formPanel.Controls.Add(MainForm.MakeLabel("Location:", lx, row3 + 6));
            txtLocation = MainForm.MakeTextBox(fx, row3, 380);
            formPanel.Controls.Add(txtLocation);

            // Message label
            lblMsg = new Label
            {
                AutoSize  = true,
                Location  = new Point(16, row3 + 44),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94)
            };
            formPanel.Controls.Add(lblMsg);

            // Action buttons
            var btnInsert = MainForm.MakeButton("➕ Add Equipment", lx + 430, row3, Color.FromArgb(34, 197, 94), 160);
            btnInsert.Click += BtnInsert_Click;
            formPanel.Controls.Add(btnInsert);

            var btnUpdate = MainForm.MakeButton("✏ Update", lx + 600, row3, AccentBlue, 110);
            btnUpdate.Click += BtnUpdate_Click;
            formPanel.Controls.Add(btnUpdate);

            var btnDelete = MainForm.MakeButton("🗑 Delete", lx + 720, row3, DangerRed, 90);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            // Grid
            grid = MainForm.CreateStyledGrid();
            grid.Bounds = new Rectangle(0, 286, this.Width - 50, this.Height - 310);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grid.SelectionChanged += Grid_SelectionChanged;
            this.Controls.Add(grid);
        }

        private void LoadYards()
        {
            cboYard.Items.Clear();
            cboYard.Items.Add("-- Select Yard --");
            var dt = DatabaseHelper.ExecuteQuery("SELECT YardID, Location FROM ServiceYard ORDER BY Location");
            foreach (DataRow r in dt.Rows)
                cboYard.Items.Add(new ComboItem(r["Location"].ToString(), (int)r["YardID"]));
            cboYard.SelectedIndex = 0;
        }

        private void LoadEquipment()
        {
            var sql =
                @"SELECT e.EquipmentID AS [ID], e.Model, e.EnginePower AS [Engine Power],
                         e.HourlyRate AS [Rate/hr], e.Location, e.Status,
                         ISNULL(sy.Location,'—') AS [Yard]
                  FROM   Equipment e
                  LEFT JOIN ServiceYard sy ON sy.YardID = e.YardID
                  ORDER BY e.EquipmentID";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void SearchEquipment()
        {
            var sql =
                @"SELECT e.EquipmentID AS [ID], e.Model, e.EnginePower AS [Engine Power],
                         e.HourlyRate AS [Rate/hr], e.Location, e.Status,
                         ISNULL(sy.Location,'—') AS [Yard]
                  FROM   Equipment e
                  LEFT JOIN ServiceYard sy ON sy.YardID = e.YardID
                  WHERE  e.Model LIKE @m
                  ORDER BY e.EquipmentID";
            var p = new[] { new SqlParameter("@m", "%" + txtSearch.Text.Trim() + "%") };
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql, p);
        }

        private void Grid_SelectionChanged(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            txtModel.Text    = row.Cells["Model"].Value?.ToString();
            txtPower.Text    = row.Cells["Engine Power"].Value?.ToString();
            txtRate.Text     = row.Cells["Rate/hr"].Value?.ToString();
            txtLocation.Text = row.Cells["Location"].Value?.ToString();
            cboStatus.SelectedItem = row.Cells["Status"].Value?.ToString();
            lblMsg.Text = "";
        }

        // ── F01 – INSERT Equipment ──────────────────────────────
        private void BtnInsert_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtRate.Text))
            { ShowMsg("Model and Hourly Rate are required.", DangerRed); return; }

            if (!decimal.TryParse(txtRate.Text, out decimal rate))
            { ShowMsg("Hourly Rate must be a number.", DangerRed); return; }

            int? yardId = GetSelectedYardId();

            var sql = @"INSERT INTO Equipment (Model, EnginePower, HourlyRate, Location, Status, YardID)
                        VALUES (@model, @power, @rate, @loc, @status, @yard)";
            var p = new[]
            {
                new SqlParameter("@model",  txtModel.Text.Trim()),
                new SqlParameter("@power",  txtPower.Text.Trim()),
                new SqlParameter("@rate",   rate),
                new SqlParameter("@loc",    txtLocation.Text.Trim()),
                new SqlParameter("@status", cboStatus.SelectedItem?.ToString() ?? "Available"),
                new SqlParameter("@yard",   (object)yardId ?? DBNull.Value)
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Equipment added successfully!", Color.FromArgb(34, 197, 94)); LoadEquipment(); ClearForm(); }
        }

        // ── F02 – UPDATE Equipment ──────────────────────────────
        private void BtnUpdate_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to update.", DangerRed); return; }
            int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);

            if (!decimal.TryParse(txtRate.Text, out decimal rate))
            { ShowMsg("Hourly Rate must be a number.", DangerRed); return; }

            int? yardId = GetSelectedYardId();

            var sql = @"UPDATE Equipment SET
                            Model       = @model,
                            EnginePower = @power,
                            HourlyRate  = @rate,
                            Location    = @loc,
                            Status      = @status,
                            YardID      = @yard
                        WHERE EquipmentID = @id";
            var p = new[]
            {
                new SqlParameter("@model",  txtModel.Text.Trim()),
                new SqlParameter("@power",  txtPower.Text.Trim()),
                new SqlParameter("@rate",   rate),
                new SqlParameter("@loc",    txtLocation.Text.Trim()),
                new SqlParameter("@status", cboStatus.SelectedItem?.ToString() ?? "Available"),
                new SqlParameter("@yard",   (object)yardId ?? DBNull.Value),
                new SqlParameter("@id",     id)
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Equipment updated!", Color.FromArgb(34, 197, 94)); LoadEquipment(); }
        }

        // ── F03 – DELETE Equipment ──────────────────────────────
        private void BtnDelete_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to delete.", DangerRed); return; }
            int id    = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);
            string model = grid.SelectedRows[0].Cells["Model"].Value?.ToString();

            var confirm = MessageBox.Show(
                $"Delete equipment:\n\"{model}\"?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            var sql = "DELETE FROM Equipment WHERE EquipmentID = @id";
            var p   = new[] { new SqlParameter("@id", id) };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Equipment deleted.", Color.FromArgb(234, 179, 8)); LoadEquipment(); ClearForm(); }
        }

        private int? GetSelectedYardId()
        {
            if (cboYard.SelectedItem is ComboItem item) return item.Value;
            return null;
        }

        private void ClearForm()
        {
            txtModel.Text = txtPower.Text = txtRate.Text = txtLocation.Text = "";
            cboStatus.SelectedIndex = 0;
            cboYard.SelectedIndex   = 0;
        }

        private void ShowMsg(string msg, Color color)
        {
            lblMsg.Text      = msg;
            lblMsg.ForeColor = color;
        }
    }

    // Helper for ComboBox items
    public class ComboItem
    {
        public string Text  { get; }
        public int    Value { get; }
        public ComboItem(string text, int value) { Text = text; Value = value; }
        public override string ToString() => Text;
    }
}
