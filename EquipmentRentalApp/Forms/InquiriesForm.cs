using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using EquipmentRentalApp.Database;

namespace EquipmentRentalApp.Forms
{
    public partial class InquiriesForm : Form
    {
        private DataGridView dgvResults;
        private Label lblTitle, lblDescription, lblMsg;
        private Panel btnPanel;

        public InquiriesForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Inquiries & Reports";
            this.Size = new Size(1100, 700);
            this.BackColor = Color.FromArgb(45, 45, 50);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Page title
            Label lblPage = new Label
            {
                Text = "📊 Inquiries & Reports",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(500, 40)
            };
            this.Controls.Add(lblPage);

            // Inquiry buttons panel
            btnPanel = new Panel
            {
                Location = new Point(20, 65),
                Size = new Size(1050, 130),
                BackColor = Color.FromArgb(35, 35, 40)
            };

            Button[] buttons = new Button[]
            {
                MakeInquiryBtn("1️⃣  Most Rented\nEquipment",       0,   RunInquiry1),
                MakeInquiryBtn("2️⃣  Inactive\nService Yards",      180, RunInquiry2),
                MakeInquiryBtn("3️⃣  Top\nTechnicians",             360, RunInquiry3),
                MakeInquiryBtn("4️⃣  Inactive\nContractors",        540, RunInquiry4),
                MakeInquiryBtn("5️⃣  Available\nEquipment by Yard", 720, RunInquiry5),
                MakeInquiryBtn("6️⃣  Contractor\nRental Hours",     900, RunInquiry6),
            };

            foreach (var btn in buttons)
                btnPanel.Controls.Add(btn);

            this.Controls.Add(btnPanel);

            // Active inquiry label
            lblTitle = new Label
            {
                Text = "Select an inquiry above to run it.",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 180, 255),
                Location = new Point(20, 210),
                Size = new Size(700, 25)
            };
            this.Controls.Add(lblTitle);

            lblDescription = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Silver,
                Location = new Point(20, 235),
                Size = new Size(1050, 20)
            };
            this.Controls.Add(lblDescription);

            // Status / error message
            lblMsg = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Tomato,
                Location = new Point(20, 258),
                Size = new Size(1050, 20)
            };
            this.Controls.Add(lblMsg);

            // Results grid
            dgvResults = new DataGridView
            {
                Location = new Point(20, 282),
                Size = new Size(1050, 380),
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };
            dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            this.Controls.Add(dgvResults);
        }

        private Button MakeInquiryBtn(string text, int x, EventHandler handler)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 80, 130),
                Size = new Size(165, 90),
                Location = new Point(x + 10, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                FlatAppearance = { BorderColor = Color.FromArgb(80, 120, 200), BorderSize = 1 }
            };
            btn.Click += handler;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(70, 110, 180);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(50, 80, 130);
            return btn;
        }

        // ─── Inquiry 1: Top 5 Most Rented Equipment ────────────────────────────
        private void RunInquiry1(object sender, EventArgs e)
        {
            lblTitle.Text = "Inquiry 1 — Top 5 Most Rented Equipment";
            lblDescription.Text = "Equipment ranked by total number of rental agreements.";

            // FIX: RentalAgreement has no TotalHours column — use TotalCost instead
            string sql = @"
                SELECT TOP 5
                    e.Model                      AS [Equipment Model],
                    COUNT(ra.AgreementID)        AS [Total Rentals],
                    SUM(ISNULL(ra.TotalCost, 0)) AS [Total Revenue ($)]
                FROM RentalAgreement ra
                JOIN Equipment e ON e.EquipmentID = ra.EquipmentID
                GROUP BY e.EquipmentID, e.Model
                ORDER BY COUNT(ra.AgreementID) DESC;";

            RunQuery(sql);
        }

        // ─── Inquiry 2: Inactive Service Yards ─────────────────────────────────
        private void RunInquiry2(object sender, EventArgs e)
        {
            lblTitle.Text = "Inquiry 2 — Inactive Service Yards (No Rentals in Last Month)";
            lblDescription.Text = "Service yards with no rental activity in the past 30 days.";

            // FIX: ServiceYard has YardName not just Location
            string sql = @"
                SELECT
                    sy.YardID    AS [Yard ID],
                    sy.YardName  AS [Yard Name],
                    sy.Location  AS [Location],
                    sy.Capacity
                FROM ServiceYard sy
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM RentalAgreement ra
                    JOIN Equipment e ON e.EquipmentID = ra.EquipmentID
                    WHERE e.YardID = sy.YardID
                      AND ra.StartDate >= DATEADD(MONTH, -1, GETDATE())
                )
                ORDER BY sy.YardName;";

            RunQuery(sql);
        }

        // ─── Inquiry 3: Top 5 Technicians by Inspections ───────────────────────
        private void RunInquiry3(object sender, EventArgs e)
        {
            lblTitle.Text = "Inquiry 3 — Top 5 Technicians (Most Inspections Last Month)";
            lblDescription.Text = "Technicians ranked by inspections completed in the past 30 days.";

            // FIX: Technician column is TechnicianName (not FullName), Specialization (not Specialty)
            string sql = @"
                SELECT TOP 5
                    t.TechnicianID                AS [ID],
                    t.TechnicianName              AS [Technician Name],
                    t.Specialization              AS [Specialization],
                    COUNT(si.InspectionID)        AS [Inspections Last Month]
                FROM SafetyInspection si
                JOIN Technician t ON t.TechnicianID = si.TechnicianID
                WHERE si.InspectionDate >= DATEADD(MONTH, -1, GETDATE())
                GROUP BY t.TechnicianID, t.TechnicianName, t.Specialization
                ORDER BY COUNT(si.InspectionID) DESC;";

            RunQuery(sql);
        }

        // ─── Inquiry 4: Inactive Contractors ───────────────────────────────────
        private void RunInquiry4(object sender, EventArgs e)
        {
            lblTitle.Text = "Inquiry 4 — Inactive Contractors (No Rentals in Last Month)";
            lblDescription.Text = "Contractors who have not made any rental agreements in the past 30 days.";

            // This one matches the DB schema exactly — no changes needed
            string sql = @"
                SELECT
                    c.ContractorID  AS [ID],
                    c.CompanyName   AS [Company],
                    c.ContactInfo   AS [Contact],
                    c.CreditLimit   AS [Credit Limit]
                FROM Contractor c
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM RentalAgreement ra
                    WHERE ra.ContractorID = c.ContractorID
                      AND ra.StartDate >= DATEADD(MONTH, -1, GETDATE())
                )
                ORDER BY c.CompanyName;";

            RunQuery(sql);
        }

        // ─── Inquiry 5: Available Equipment by Yard ─────────────────────────────
        private void RunInquiry5(object sender, EventArgs e)
        {
            lblTitle.Text = "Inquiry 5 — Available Equipment by Service Yard";
            lblDescription.Text = "All equipment currently marked Available, grouped by their assigned service yard.";

            // This one matches the DB schema — no changes needed
            string sql = @"
                SELECT
                    sy.YardName   AS [Yard Name],
                    sy.Location   AS [Yard Location],
                    e.Model       AS [Equipment Model],
                    e.EnginePower AS [Engine Power],
                    e.HourlyRate  AS [Rate/hr ($)],
                    e.Status
                FROM Equipment e
                JOIN ServiceYard sy ON sy.YardID = e.YardID
                WHERE e.Status = 'Available'
                ORDER BY sy.YardName, e.Model;";

            RunQuery(sql);
        }

        // ─── Inquiry 6: Contractor Rental Hours ────────────────────────────────
        private void RunInquiry6(object sender, EventArgs e)
        {
            lblTitle.Text = "Inquiry 6 — Contractor Rental Hours (Last Month)";
            lblDescription.Text = "Total hours and cost each contractor has rented equipment in the past 30 days.";

            // FIX: added TotalCost sum; DATEDIFF on DATE columns uses DATEDIFF(DAY)*24 for hour approx
            string sql = @"
                SELECT
                    c.CompanyName                                                        AS [Company],
                    c.CreditLimit                                                        AS [Credit Limit],
                    COUNT(ra.AgreementID)                                                AS [Agreements],
                    SUM(DATEDIFF(HOUR, ra.StartDate, ISNULL(ra.EndDate, GETDATE())))    AS [Total Hours Rented],
                    SUM(ISNULL(ra.TotalCost, 0))                                         AS [Total Cost ($)]
                FROM Contractor c
                LEFT JOIN RentalAgreement ra
                    ON ra.ContractorID = c.ContractorID
                    AND ra.StartDate >= DATEADD(MONTH, -1, GETDATE())
                GROUP BY c.ContractorID, c.CompanyName, c.CreditLimit
                ORDER BY SUM(DATEDIFF(HOUR, ra.StartDate, ISNULL(ra.EndDate, GETDATE()))) DESC;";

            RunQuery(sql);
        }

        // ─── Shared query runner ────────────────────────────────────────────────
        private void RunQuery(string sql)
        {
            lblMsg.Text = "";
            dgvResults.DataSource = null;
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvResults.DataSource = dt;

                if (dt.Rows.Count == 0)
                    lblMsg.Text = "✓ Query ran successfully — no rows matched.";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.ForeColor = Color.Tomato;
            }
        }
    }
}