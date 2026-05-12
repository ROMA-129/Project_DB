using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class InspectionForm : Form
    {
        static readonly Color BgDark      = Color.FromArgb(15, 23, 42);
        static readonly Color BgCard      = Color.FromArgb(30, 41, 59);
        static readonly Color AccentBlue  = Color.FromArgb(59, 130, 246);
        static readonly Color TextLight   = Color.FromArgb(226, 232, 240);
        static readonly Color TextMuted   = Color.FromArgb(148, 163, 184);
        static readonly Color DangerRed   = Color.FromArgb(239, 68, 68);
        static readonly Color AccentGreen = Color.FromArgb(34, 197, 94);
        static readonly Color AccentGold  = Color.FromArgb(234, 179, 8);

        private DataGridView grid;
        private ComboBox cboEquipment, cboTechnician, cboResult;
        private DateTimePicker dtpDate;
        private TextBox txtNotes;
        private Label lblMsg;

        public InspectionForm() { InitializeUI(); LoadInspections(); }

        private void InitializeUI()
        {
            this.BackColor = BgDark;
            this.Padding   = new Padding(20);

            this.Controls.Add(MainForm.MakeLabel("Safety Inspection Management", 0, 0, 18, true, AccentBlue));
            this.Controls.Add(MainForm.MakeLabel("Log safety inspections for equipment before/after rental", 0, 34, 10, false, TextMuted));

            var formPanel = new Panel
            {
                Bounds    = new Rectangle(0, 70, 860, 175),
                BackColor = BgCard
            };
            this.Controls.Add(formPanel);

            int lx = 16, fx = 150, r1 = 14, r2 = 62, r3 = 110;

            formPanel.Controls.Add(MainForm.MakeLabel("Equipment:", lx, r1 + 6));
            cboEquipment = MainForm.MakeCombo(fx, r1, 260);
            formPanel.Controls.Add(cboEquipment);

            formPanel.Controls.Add(MainForm.MakeLabel("Technician:", lx + 430, r1 + 6));
            cboTechnician = MainForm.MakeCombo(fx + 430, r1, 260);
            formPanel.Controls.Add(cboTechnician);

            formPanel.Controls.Add(MainForm.MakeLabel("Inspection Date:", lx, r2 + 6));
            dtpDate = new DateTimePicker
            {
                Bounds = new Rectangle(fx, r2, 190, 30),
                Format = DateTimePickerFormat.Short,
                Value  = DateTime.Today
            };
            formPanel.Controls.Add(dtpDate);

            formPanel.Controls.Add(MainForm.MakeLabel("Result:", lx + 310, r2 + 6));
            cboResult = MainForm.MakeCombo(fx + 310, r2, 130);
            cboResult.Items.AddRange(new[] { "Pass", "Fail", "Pending" });
            cboResult.SelectedIndex = 0;
            formPanel.Controls.Add(cboResult);

            formPanel.Controls.Add(MainForm.MakeLabel("Notes:", lx, r3 + 6));
            txtNotes = MainForm.MakeTextBox(fx, r3, 500);
            formPanel.Controls.Add(txtNotes);

            lblMsg = new Label
            {
                AutoSize  = true,
                Location  = new Point(lx, r3 + 44),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = AccentGreen
            };
            formPanel.Controls.Add(lblMsg);

            var btnLog = MainForm.MakeButton("🛡 Log Inspection", 620, r3, AccentGreen, 160);
            btnLog.Click += BtnInsert_Click;
            formPanel.Controls.Add(btnLog);

            var btnDelete = MainForm.MakeButton("🗑 Delete", 788, r3, DangerRed, 90);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            grid = MainForm.CreateStyledGrid();
            grid.Bounds = new Rectangle(0, 257, this.Width - 50, this.Height - 272);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(grid);

            LoadCombos();
        }

        private void LoadCombos()
        {
            cboEquipment.Items.Clear();
            cboEquipment.Items.Add("-- Select Equipment --");
            var eqDt = DatabaseHelper.ExecuteQuery("SELECT EquipmentID, Model FROM Equipment ORDER BY Model");
            foreach (DataRow r in eqDt.Rows)
                cboEquipment.Items.Add(new ComboItem(r["Model"].ToString(), (int)r["EquipmentID"]));
            cboEquipment.SelectedIndex = 0;

            cboTechnician.Items.Clear();
            cboTechnician.Items.Add("-- Select Technician --");
            var techDt = DatabaseHelper.ExecuteQuery("SELECT TechnicianID, FullName FROM Technician ORDER BY FullName");
            foreach (DataRow r in techDt.Rows)
                cboTechnician.Items.Add(new ComboItem(r["FullName"].ToString(), (int)r["TechnicianID"]));
            cboTechnician.SelectedIndex = 0;
        }

        private void LoadInspections()
        {
            var sql =
                @"SELECT si.InspectionID AS [ID],
                         e.Model          AS [Equipment],
                         t.FullName        AS [Technician],
                         si.InspectionDate AS [Date],
                         si.Result,
                         si.Notes
                  FROM   SafetyInspection si
                  JOIN   Equipment   e ON e.EquipmentID  = si.EquipmentID
                  JOIN   Technician  t ON t.TechnicianID = si.TechnicianID
                  ORDER BY si.InspectionDate DESC";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ── F10 – INSERT SafetyInspection ──────────────────────
        private void BtnInsert_Click(object s, EventArgs e)
        {
            if (!(cboEquipment.SelectedItem is ComboItem eq))
            { ShowMsg("Select equipment.", DangerRed); return; }
            if (!(cboTechnician.SelectedItem is ComboItem tech))
            { ShowMsg("Select technician.", DangerRed); return; }

            var sql = @"INSERT INTO SafetyInspection (EquipmentID, TechnicianID, InspectionDate, Result, Notes)
                        VALUES (@eq, @tech, @date, @result, @notes)";
            var p = new[]
            {
                new SqlParameter("@eq",     eq.Value),
                new SqlParameter("@tech",   tech.Value),
                new SqlParameter("@date",   dtpDate.Value.Date),
                new SqlParameter("@result", cboResult.SelectedItem?.ToString() ?? "Pending"),
                new SqlParameter("@notes",  txtNotes.Text.Trim())
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Inspection logged successfully!", AccentGreen); LoadInspections(); txtNotes.Text = ""; }
        }

        private void BtnDelete_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to delete.", DangerRed); return; }
            int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);

            var confirm = MessageBox.Show("Delete this inspection record?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            int rows = DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM SafetyInspection WHERE InspectionID = @id",
                new[] { new SqlParameter("@id", id) });
            if (rows > 0) { ShowMsg("Inspection deleted.", AccentGold); LoadInspections(); }
        }

        private void ShowMsg(string msg, Color color) { lblMsg.Text = msg; lblMsg.ForeColor = color; }
    }
}
