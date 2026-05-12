using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class ContractorForm : Form
    {
        static readonly Color BgDark    = Color.FromArgb(15, 23, 42);
        static readonly Color BgCard    = Color.FromArgb(30, 41, 59);
        static readonly Color AccentBlue= Color.FromArgb(59, 130, 246);
        static readonly Color TextLight = Color.FromArgb(226, 232, 240);
        static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
        static readonly Color DangerRed = Color.FromArgb(239, 68, 68);
        static readonly Color AccentGreen = Color.FromArgb(34, 197, 94);

        private DataGridView grid;
        private TextBox txtCompany, txtContact, txtCredit;
        private Label lblMsg;

        public ContractorForm() { InitializeUI(); LoadContractors(); }

        private void InitializeUI()
        {
            this.BackColor = BgDark;
            this.Padding   = new Padding(20);

            this.Controls.Add(MainForm.MakeLabel("Contractor Management", 0, 0, 18, true, AccentBlue));
            this.Controls.Add(MainForm.MakeLabel("Register, update, or remove contractor profiles", 0, 34, 10, false, TextMuted));

            // Form Panel
            var formPanel = new Panel
            {
                Bounds    = new Rectangle(0, 70, 700, 150),
                BackColor = BgCard
            };
            this.Controls.Add(formPanel);

            int lx = 16, fx = 160, r1 = 16, r2 = 64;

            formPanel.Controls.Add(MainForm.MakeLabel("Company Name:", lx, r1 + 6));
            txtCompany = MainForm.MakeTextBox(fx, r1, 260);
            formPanel.Controls.Add(txtCompany);

            formPanel.Controls.Add(MainForm.MakeLabel("Credit Limit (EGP):", lx + 440, r1 + 6));
            txtCredit = MainForm.MakeTextBox(fx + 440, r1, 180);
            formPanel.Controls.Add(txtCredit);

            formPanel.Controls.Add(MainForm.MakeLabel("Contact Info:", lx, r2 + 6));
            txtContact = MainForm.MakeTextBox(fx, r2, 480);
            formPanel.Controls.Add(txtContact);

            lblMsg = new Label
            {
                AutoSize  = true,
                Location  = new Point(lx, r2 + 44),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = AccentGreen
            };
            formPanel.Controls.Add(lblMsg);

            var btnAdd = MainForm.MakeButton("➕ Add Contractor", 450, r2, AccentGreen, 160);
            btnAdd.Click += BtnInsert_Click;
            formPanel.Controls.Add(btnAdd);

            var btnUpdate = MainForm.MakeButton("✏ Update", 620, r2, AccentBlue, 100);
            btnUpdate.Click += BtnUpdate_Click;
            formPanel.Controls.Add(btnUpdate);

            var btnDelete = MainForm.MakeButton("🗑 Delete", 728, r2, DangerRed, 90);
            btnDelete.Click += BtnDelete_Click;
            formPanel.Controls.Add(btnDelete);

            // Grid
            grid = MainForm.CreateStyledGrid();
            grid.Bounds = new Rectangle(0, 232, this.Width - 50, this.Height - 250);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            grid.SelectionChanged += Grid_SelectionChanged;
            this.Controls.Add(grid);
        }

        private void LoadContractors()
        {
            var sql =
                @"SELECT c.ContractorID AS [ID],
                         c.CompanyName  AS [Company],
                         c.ContactInfo  AS [Contact],
                         c.CreditLimit  AS [Credit Limit (EGP)],
                         COUNT(ra.AgreementID) AS [Total Rentals]
                  FROM   Contractor c
                  LEFT JOIN RentalAgreement ra ON ra.ContractorID = c.ContractorID
                  GROUP BY c.ContractorID, c.CompanyName, c.ContactInfo, c.CreditLimit
                  ORDER BY c.CompanyName";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void Grid_SelectionChanged(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            txtCompany.Text = row.Cells["Company"].Value?.ToString();
            txtContact.Text = row.Cells["Contact"].Value?.ToString();
            txtCredit.Text  = row.Cells["Credit Limit (EGP)"].Value?.ToString();
            lblMsg.Text = "";
        }

        // ── F04 – INSERT Contractor ─────────────────────────────
        private void BtnInsert_Click(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompany.Text))
            { ShowMsg("Company name is required.", DangerRed); return; }

            if (!decimal.TryParse(txtCredit.Text, out decimal credit))
            { ShowMsg("Credit Limit must be a number.", DangerRed); return; }

            var sql = @"INSERT INTO Contractor (CompanyName, ContactInfo, CreditLimit)
                        VALUES (@company, @contact, @credit)";
            var p = new[]
            {
                new SqlParameter("@company", txtCompany.Text.Trim()),
                new SqlParameter("@contact", txtContact.Text.Trim()),
                new SqlParameter("@credit",  credit)
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Contractor added successfully!", AccentGreen); LoadContractors(); ClearForm(); }
        }

        // ── F05 – UPDATE Contractor ─────────────────────────────
        private void BtnUpdate_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to update.", DangerRed); return; }
            int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);

            if (!decimal.TryParse(txtCredit.Text, out decimal credit))
            { ShowMsg("Credit Limit must be a number.", DangerRed); return; }

            var sql = @"UPDATE Contractor SET
                            CompanyName = @company,
                            ContactInfo = @contact,
                            CreditLimit = @credit
                        WHERE ContractorID = @id";
            var p = new[]
            {
                new SqlParameter("@company", txtCompany.Text.Trim()),
                new SqlParameter("@contact", txtContact.Text.Trim()),
                new SqlParameter("@credit",  credit),
                new SqlParameter("@id",      id)
            };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Contractor updated!", AccentGreen); LoadContractors(); }
        }

        // ── F06 – DELETE Contractor ─────────────────────────────
        private void BtnDelete_Click(object s, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { ShowMsg("Select a row to delete.", DangerRed); return; }
            int id      = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);
            string name = grid.SelectedRows[0].Cells["Company"].Value?.ToString();

            // Check for active agreements (condition-based delete)
            var checkSql = "SELECT COUNT(*) FROM RentalAgreement WHERE ContractorID=@id AND ReturnStatus='Active'";
            var checkP   = new[] { new SqlParameter("@id", id) };
            int active   = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkSql, checkP));
            if (active > 0)
            { ShowMsg($"Cannot delete — contractor has {active} active rental(s).", DangerRed); return; }

            var confirm = MessageBox.Show($"Delete contractor:\n\"{name}\"?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            var sql = "DELETE FROM Contractor WHERE ContractorID = @id";
            var p   = new[] { new SqlParameter("@id", id) };
            int rows = DatabaseHelper.ExecuteNonQuery(sql, p);
            if (rows > 0) { ShowMsg("Contractor deleted.", Color.FromArgb(234, 179, 8)); LoadContractors(); ClearForm(); }
        }

        private void ClearForm()
        {
            txtCompany.Text = txtContact.Text = txtCredit.Text = "";
        }

        private void ShowMsg(string msg, Color color)
        {
            lblMsg.Text      = msg;
            lblMsg.ForeColor = color;
        }
    }
}
