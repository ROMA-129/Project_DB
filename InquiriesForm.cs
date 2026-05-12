using System;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class InquiriesForm : Form
    {
        static readonly Color BgDark     = Color.FromArgb(15, 23, 42);
        static readonly Color BgCard     = Color.FromArgb(30, 41, 59);
        static readonly Color AccentBlue = Color.FromArgb(59, 130, 246);
        static readonly Color AccentGold = Color.FromArgb(234, 179, 8);
        static readonly Color TextLight  = Color.FromArgb(226, 232, 240);
        static readonly Color TextMuted  = Color.FromArgb(148, 163, 184);

        private DataGridView grid;
        private Label lblQueryTitle, lblQueryDesc;
        private Panel btnPanel;

        public InquiriesForm() { InitializeUI(); }

        private void InitializeUI()
        {
            this.BackColor = BgDark;
            this.Padding   = new Padding(20);

            this.Controls.Add(MainForm.MakeLabel("Inquiries & Reports", 0, 0, 18, true, AccentGold));
            this.Controls.Add(MainForm.MakeLabel("Select any inquiry to run the corresponding SQL query", 0, 34, 10, false, TextMuted));

            // Buttons panel
            btnPanel = new Panel
            {
                Bounds    = new Rectangle(0, 68, this.Width, 52),
                BackColor = BgCard,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(btnPanel);

            AddInquiryButton("📊 1. Most Rented", 8,   () => RunInquiry1());
            AddInquiryButton("🏭 2. Inactive Yards", 170, () => RunInquiry2());
            AddInquiryButton("🛡 3. Top Technician", 332, () => RunInquiry3());
            AddInquiryButton("🤝 4. Inactive Contractors", 494, () => RunInquiry4());
            AddInquiryButton("📦 5. Equipment per Yard", 656, () => RunInquiry5());
            AddInquiryButton("⏱ 6. Contractor Hours", 818, () => RunInquiry6());

            // Query description box
            lblQueryTitle = new Label
            {
                Text      = "Click an inquiry button above to run a query.",
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = AccentGold,
                AutoSize  = true,
                Location  = new Point(0, 132)
            };
            this.Controls.Add(lblQueryTitle);

            lblQueryDesc = new Label
            {
                Text      = "",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = TextMuted,
                AutoSize  = true,
                Location  = new Point(0, 156)
            };
            this.Controls.Add(lblQueryDesc);

            // Grid
            grid = MainForm.CreateStyledGrid();
            grid.Bounds = new Rectangle(0, 182, this.Width - 50, this.Height - 200);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(grid);
        }

        private void AddInquiryButton(string text, int x, Action onClick)
        {
            var btn = new Button
            {
                Text      = text,
                Bounds    = new Rectangle(x, 8, 158, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 58, 100),
                ForeColor = TextLight,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = AccentBlue;
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.MouseOverBackColor = AccentBlue;
            btn.Click += (s, e) => onClick();
            btnPanel.Controls.Add(btn);
        }

        // ── F15 – Inquiry 1: Most Rented Equipment Model ─────────
        private void RunInquiry1()
        {
            lblQueryTitle.Text = "Inquiry 1: Most Rented Equipment Model (Highest Rental Agreements)";
            lblQueryDesc.Text  = "Business Goal: Identify which equipment type generates the most demand — potential candidate for inventory expansion.";

            var sql =
                @"SELECT TOP 5
                         e.Model                        AS [Equipment Model],
                         COUNT(ra.AgreementID)          AS [Total Rentals],
                         SUM(ISNULL(ra.TotalHours,0))   AS [Total Hours Rented]
                  FROM   RentalAgreement ra
                  JOIN   Equipment e ON e.EquipmentID = ra.EquipmentID
                  GROUP  BY e.EquipmentID, e.Model
                  ORDER  BY COUNT(ra.AgreementID) DESC";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ── F16 – Inquiry 2: Service Yards with No Rentals ───────
        private void RunInquiry2()
        {
            lblQueryTitle.Text = "Inquiry 2: Service Yards with No Rental Agreements (Last Month)";
            lblQueryDesc.Text  = "Business Goal: Detect underutilized yards with zero rental departures in the previous month.";

            var sql =
                @"SELECT sy.YardID    AS [Yard ID],
                         sy.Location  AS [Yard Location],
                         sy.Capacity
                  FROM   ServiceYard sy
                  WHERE  NOT EXISTS (
                      SELECT 1
                      FROM   RentalAgreement ra
                      JOIN   Equipment e ON e.EquipmentID = ra.EquipmentID
                      WHERE  e.YardID = sy.YardID
                        AND  ra.StartDate >= DATEADD(MONTH, -1, GETDATE())
                  )";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ── F17 – Inquiry 3: Top Technician by Inspections ───────
        private void RunInquiry3()
        {
            lblQueryTitle.Text = "Inquiry 3: Technician with Most Safety Inspections (Last Month)";
            lblQueryDesc.Text  = "Business Goal: Recognize top-performing technicians with the highest workload.";

            var sql =
                @"SELECT TOP 5
                         t.TechnicianID   AS [ID],
                         t.FullName       AS [Technician Name],
                         t.Specialty,
                         COUNT(si.InspectionID) AS [Inspections Last Month]
                  FROM   SafetyInspection si
                  JOIN   Technician t ON t.TechnicianID = si.TechnicianID
                  WHERE  si.InspectionDate >= DATEADD(MONTH, -1, GETDATE())
                  GROUP  BY t.TechnicianID, t.FullName, t.Specialty
                  ORDER  BY COUNT(si.InspectionID) DESC";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ── F18 – Inquiry 4: Contractors with No New Agreements ──
        private void RunInquiry4()
        {
            lblQueryTitle.Text = "Inquiry 4: Contractors with No New Rental Agreements (Last Month)";
            lblQueryDesc.Text  = "Business Goal: Identify past clients who have not initiated new rentals — targeted for CRM follow-up.";

            var sql =
                @"SELECT c.ContractorID  AS [ID],
                         c.CompanyName   AS [Company],
                         c.ContactInfo   AS [Contact],
                         c.CreditLimit   AS [Credit Limit]
                  FROM   Contractor c
                  WHERE  NOT EXISTS (
                      SELECT 1
                      FROM   RentalAgreement ra
                      WHERE  ra.ContractorID = c.ContractorID
                        AND  ra.StartDate >= DATEADD(MONTH, -1, GETDATE())
                  )
                  ORDER  BY c.CompanyName";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ── F19 – Inquiry 5: Available Equipment per Yard ────────
        private void RunInquiry5()
        {
            lblQueryTitle.Text = "Inquiry 5: Available Equipment at Each Service Yard";
            lblQueryDesc.Text  = "Business Goal: Real-time inventory report showing ready-to-rent equipment per location.";

            var sql =
                @"SELECT sy.Location   AS [Yard Location],
                         e.Model       AS [Equipment Model],
                         e.EnginePower AS [Engine Power],
                         e.HourlyRate  AS [Rate/hr],
                         e.Status
                  FROM   Equipment e
                  JOIN   ServiceYard sy ON sy.YardID = e.YardID
                  WHERE  e.Status = 'Available'
                  ORDER  BY sy.Location, e.Model";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        // ── F20 – Inquiry 6: Total Rental Hours per Contractor ───
        private void RunInquiry6()
        {
            lblQueryTitle.Text = "Inquiry 6: Total Rental Hours per Contractor (Last Month)";
            lblQueryDesc.Text  = "Business Goal: Financial summary of rental duration per company — credit and utilization analysis.";

            var sql =
                @"SELECT c.CompanyName  AS [Company],
                         c.CreditLimit  AS [Credit Limit],
                         COUNT(ra.AgreementID)                       AS [Agreements],
                         SUM(DATEDIFF(HOUR,
                               ra.StartDate,
                               ISNULL(ra.EndDate, GETDATE())))       AS [Total Hours Rented]
                  FROM   Contractor c
                  LEFT JOIN RentalAgreement ra ON ra.ContractorID = c.ContractorID
                      AND ra.StartDate >= DATEADD(MONTH, -1, GETDATE())
                  GROUP  BY c.ContractorID, c.CompanyName, c.CreditLimit
                  ORDER  BY SUM(DATEDIFF(HOUR, ra.StartDate,
                               ISNULL(ra.EndDate, GETDATE()))) DESC";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }
    }
}
