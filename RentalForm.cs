using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class RentalForm : Form
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
        private ComboBox cboEquipment, cboContractor, cboStatus;
        private DateTimePicker dtpStart, dtpEnd;
        private CheckBox chkEndDate;
        private Label lblMsg;

        public RentalForm() { InitializeUI(); LoadAgreements(); }

        private void InitializeUI()
        {
            this.BackColor = BgDark;
            this.Padding   = new Padding(20);

            this.Controls.Add(MainForm.MakeLabel("Rental Agreement Management", 0, 0, 18, true, AccentBlue));
            this.Controls.Add(MainForm.MakeLabel("Create, update, or cancel rental agreements (Equipment ↔ Contractor)", 0, 34, 10, false, TextMuted));

            var formPanel = new Panel
            {
                Bounds    = new Rectangle(0, 70, 840, 170),
                BackColor = BgCard
            };
            this.Controls.Add(formPanel);

            int lx = 16, fx = 160, r1 = 14, r2 = 62, r3 = 110;

            // Row 1
            formPanel.Controls.Add(MainForm.MakeLabel("Equipment:", lx, r1 + 6));
            cboEquipment = MainForm.MakeCombo(fx, r1, 280);
            formPanel.Controls.Add(cboEquipment);

            formPanel.Controls.Add(MainForm.MakeLabel("Contractor:", lx + 460, r1 + 6));
            cboContractor = MainForm.MakeCombo(fx + 460, r1, 260);
            formPanel.Controls.Add(cboContractor);

            // Row 2
            formPanel.Controls.Add(MainForm.MakeLabel("Start Date:", lx, r2 + 6));
            dtpStart = new DateTimePicker
            {
                Bounds    = new Rectangle(fx, r2, 200, 30),
                Format    = DateTimePickerFormat.Short,
                Value     = DateTime.Today
            };
            formPanel.Controls.Add(dtpStart);

            chkEndDate = new CheckBox
            {
                Text      = "Set End Date:",
                Location  = new Point(lx + 390, r2 + 5),
                AutoSize  = true,
                ForeColor = TextLight,
                Font      = new Font("Segoe UI", 9.5f)
            };
            formPanel.Controls.Add(chkEndDate);

            dtpEnd = new DateTimePicker
            {
                Bounds  = new Rectangle(fx + 390, r2, 200, 30),
                Format  = DateTimePickerFormat.Short,
                Value   = DateTime.Today.AddDays(7),
                Enabled = false
            };
            chkEndDate.CheckedChanged += (s, e) => dtpEnd.Enabled = chkEndDate.Checked;
            formPanel.Controls.Add(dtpEnd);

            formPanel.Controls.Add(MainForm.MakeLabel("Status:", lx + 640, r2 + 6));
            cboStatus = MainForm.MakeCombo(fx + 640, r2, 140);
            cboStatus.Items.AddRange(new[] { "Active", "Returned", "Cancelled" });
            cboStatus.SelectedIndex = 0;
            formPanel.Controls.Add(cboStatus);

            // Message
            lblMsg = new Label
            {
                AutoSize  = true,
                Location  = new Point(lx, r3 + 6),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = AccentGreen
            };
            formPanel.Controls.Add(lblMsg);

            // Buttons
            var btnCreate = MainForm.MakeButton("➕ Create Agreement", 400, r3, AccentGreen, 170);
            btnCreate.Click += BtnCreate_Click;
            formPanel.Controls.Add(btnCreate);

            var btnUpdate = MainForm.MakeButton("✏ Update Status", 580, r3, AccentBlue, 150);
            btnUpdate.Click += BtnUpdate_Click;
            formPanel.Controls.Add(btnUpdate);

            var btnDelete = MainForm.MakeButton("🗑 Cancel", 738, r3, DangerRed, 90);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            // Grid
            grid = MainForm.CreateStyledGrid();
            grid.Bounds = new Rectangle(0, 252, this.Width - 50, this.Height - 268);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grid.SelectionChanged += Grid_SelectionChanged;
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

            cboContractor.Items.Clear();
            cboContractor.Items.Add("-- Select Contractor --");
            var coDt = DatabaseHelper.ExecuteQuery("SELECT ContractorID, CompanyName FROM Contractor ORDER BY CompanyName");
            foreach (DataRow r in coDt.Rows)
                cboContractor.Items.Add(new ComboItem(r["CompanyName"].ToString(), (int)r["ContractorID"]));
            cboContractor.SelectedIndex = 0;
        }

        private void LoadAgreements()
        {
            var sql =
                @"SELECT ra.AgreementID AS [ID],
                         e.Model        AS [Equipment],
                         c.CompanyName  AS [Contractor],
                         ra.StartDate   AS [Start],
                         ISNULL(CONVERT(VARCHAR,ra.EndDate,23),'—') AS [End],
                         ra.ReturnStatus AS [Status],
                         ISNULL(CAST(ra.TotalHours AS VARCHAR),'—') AS [Hours]
                  FROM   RentalAgreement ra
                  JOIN   Equipment  e ON e.EquipmentID  = ra.EquipmentID
                  JOIN   Contractor c ON c.ContractorID = ra.ContractorID
                  ORDER BY ra.AgreementID DESC";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void Grid_SelectionChanged(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            string status = row.Cells["Status"].Value?.ToString();
            if (status != null)
            {
                cboStatus.SelectedItem = status;
            }
            lblMsg.Text = "";
        }

        // ── F07 – INSERT RentalAgreement ────────────────────────
        private void BtnCreate_Click(object s, EventArgs e)
        {
            if (!(cboEquipment.SelectedItem is ComboItem eq))
            { ShowMsg("Select equipment.", DangerRed); return; }
            if (!(cboContractor.SelectedItem is ComboItem co))
            { ShowMsg("Select contractor.", DangerRed); return; }
            if (chkEndDate.Checked && dtpEnd.Value <= dtpStart.Value)
            { ShowMsg("End date must be after start date.", DangerRed); return; }

            var sql = @"INSERT INTO RentalAgreement (EquipmentID, ContractorID, StartDate, EndDate, ReturnStatus)
                        VALUES (@eq, @co, @start, @end, @status)";
            var p = new[]
            {
                new SqlParameter("@eq",     eq.Value),
                new SqlParameter("@co",     co.Value),
                new SqlParameter("@start",  dtpStart.Value.Date),
                new SqlParameter("@end",    chkEndDate.Checked ? (object)dtpEnd.Value.Date : DBNull.Value),
                new SqlParameter("@status", cboStatus.SelectedItem?.ToString() ?? "Active")
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Agreement created!", AccentGreen); LoadAgreements(); }
        }

        // ── F08 – UPDATE RentalAgreement ────────────────────────
        private void BtnUpdate_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to update.", DangerRed); return; }
            int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);

            var sql = @"UPDATE RentalAgreement SET
                            StartDate    = @start,
                            EndDate      = @end,
                            ReturnStatus = @status
                        WHERE AgreementID = @id";
            var p = new[]
            {
                new SqlParameter("@start",  dtpStart.Value.Date),
                new SqlParameter("@end",    chkEndDate.Checked ? (object)dtpEnd.Value.Date : DBNull.Value),
                new SqlParameter("@status", cboStatus.SelectedItem?.ToString() ?? "Active"),
                new SqlParameter("@id",     id)
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Agreement updated!", AccentGreen); LoadAgreements(); }
        }

        // ── F09 – DELETE RentalAgreement ────────────────────────
        private void BtnDelete_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to cancel.", DangerRed); return; }
            int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);

            var confirm = MessageBox.Show("Cancel/delete this rental agreement?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            int rows = DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM RentalAgreement WHERE AgreementID = @id",
                new[] { new SqlParameter("@id", id) });
            if (rows > 0) { ShowMsg("Agreement deleted.", AccentGold); LoadAgreements(); }
        }

        private void ShowMsg(string msg, Color color) { lblMsg.Text = msg; lblMsg.ForeColor = color; }
    }
}
