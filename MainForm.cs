using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EquipmentRentalSystem
{
    public class MainForm : Form
    {
        // ── Colors ──────────────────────────────────────────────
        static readonly Color BgDark      = Color.FromArgb(15, 23, 42);
        static readonly Color BgCard      = Color.FromArgb(30, 41, 59);
        static readonly Color AccentBlue  = Color.FromArgb(59, 130, 246);
        static readonly Color AccentGold  = Color.FromArgb(234, 179, 8);
        static readonly Color TextLight   = Color.FromArgb(226, 232, 240);
        static readonly Color TextMuted   = Color.FromArgb(148, 163, 184);

        private Panel sidePanel;
        private Panel contentPanel;
        private Label lblTitle;

        public MainForm()
        {
            InitializeUI();
            CheckDatabaseConnection();
        }

        private void CheckDatabaseConnection()
        {
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show(
                    "Could not connect to the database.\n\n" +
                    "Please make sure:\n" +
                    "1. SQL Server is running\n" +
                    "2. The EquipmentRentalDB database is created\n" +
                    "3. The connection string in DatabaseHelper.cs is correct",
                    "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeUI()
        {
            this.Text            = "Industrial Equipment Rental & Service Yard";
            this.Size            = new Size(1280, 780);
            this.MinimumSize     = new Size(1100, 680);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = BgDark;
            this.Font            = new Font("Segoe UI", 9.5f);
            this.DoubleBuffered  = true;

            BuildSidebar();
            BuildContentPanel();
            ShowDashboard();
        }

        // ── Sidebar ─────────────────────────────────────────────
        private void BuildSidebar()
        {
            sidePanel = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 240,
                BackColor = BgCard,
                Padding   = new Padding(0)
            };
            this.Controls.Add(sidePanel);

            // Logo area
            var logoPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 90,
                BackColor = AccentBlue
            };
            sidePanel.Controls.Add(logoPanel);

            var logoLabel = new Label
            {
                Text      = "⚙  ERS",
                Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoPanel.Controls.Add(logoLabel);

            var subLabel = new Label
            {
                Text      = "Equipment Rental System",
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                AutoSize  = false,
                Dock      = DockStyle.Bottom,
                Height    = 20,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoPanel.Controls.Add(subLabel);

            // Nav buttons
            int y = 100;
            AddNavButton("🏠  Dashboard",     y,       () => ShowDashboard());
            AddNavButton("🚧  Equipment",      y + 52,  () => ShowPanel(new EquipmentForm()));
            AddNavButton("🏭  Service Yards",  y + 104, () => ShowPanel(new ServiceYardForm()));
            AddNavButton("🤝  Contractors",    y + 156, () => ShowPanel(new ContractorForm()));
            AddNavButton("📋  Rental Agreements", y + 208, () => ShowPanel(new RentalForm()));
            AddNavButton("🛡  Safety Inspections", y + 260, () => ShowPanel(new InspectionForm()));
            AddNavButton("📊  Inquiries / Reports", y + 330, () => ShowPanel(new InquiriesForm()),
                AccentGold, Color.FromArgb(20, 20, 0));

            // Footer
            var footer = new Label
            {
                Text      = "Project 19 – IS211\nThe Insight Team",
                Font      = new Font("Segoe UI", 8f),
                ForeColor = TextMuted,
                AutoSize  = false,
                Dock      = DockStyle.Bottom,
                Height    = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };
            sidePanel.Controls.Add(footer);
        }

        private void AddNavButton(string text, int y, Action onClick,
            Color? accentOverride = null, Color? bgOverride = null)
        {
            var btn = new Button
            {
                Text      = text,
                Bounds    = new Rectangle(8, y, 224, 44),
                FlatStyle = FlatStyle.Flat,
                BackColor = bgOverride ?? Color.Transparent,
                ForeColor = TextLight,
                Font      = new Font("Segoe UI", 10f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(14, 0, 0, 0),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize      = 0;
            btn.FlatAppearance.MouseOverBackColor  = Color.FromArgb(50, accentOverride ?? AccentBlue);
            btn.FlatAppearance.MouseDownBackColor  = Color.FromArgb(80, accentOverride ?? AccentBlue);
            btn.Click += (s, e) => onClick();
            sidePanel.Controls.Add(btn);
        }

        // ── Content Area ─────────────────────────────────────────
        private void BuildContentPanel()
        {
            contentPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgDark,
                Padding   = new Padding(24)
            };
            this.Controls.Add(contentPanel);
        }

        public void ShowPanel(Form childForm)
        {
            contentPanel.Controls.Clear();

            childForm.TopLevel      = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock          = DockStyle.Fill;
            childForm.BackColor     = BgDark;

            contentPanel.Controls.Add(childForm);
            childForm.Show();
        }

        // ── Dashboard ────────────────────────────────────────────
        private void ShowDashboard()
        {
            contentPanel.Controls.Clear();

            var dash = new Panel { Dock = DockStyle.Fill, BackColor = BgDark };
            contentPanel.Controls.Add(dash);

            var title = new Label
            {
                Text      = "Dashboard",
                Font      = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = TextLight,
                AutoSize  = true,
                Location  = new Point(0, 0)
            };
            dash.Controls.Add(title);

            var sub = new Label
            {
                Text      = "Industrial Equipment Rental & Service Yard — Overview",
                Font      = new Font("Segoe UI", 10f),
                ForeColor = TextMuted,
                AutoSize  = true,
                Location  = new Point(0, 34)
            };
            dash.Controls.Add(sub);

            // Stat cards
            int cardW = 200, cardH = 110, cardX = 0, cardY = 75;
            AddStatCard(dash, "Equipment", GetCount("Equipment"),    AccentBlue,  cardX,        cardY, cardW, cardH);
            AddStatCard(dash, "Contractors", GetCount("Contractor"), AccentGold,  cardX + 220,  cardY, cardW, cardH);
            AddStatCard(dash, "Active Rentals",
                GetCount("RentalAgreement", "ReturnStatus='Active'"),
                Color.FromArgb(34,197,94),  cardX + 440, cardY, cardW, cardH);
            AddStatCard(dash, "Service Yards", GetCount("ServiceYard"),
                Color.FromArgb(168,85,247), cardX + 660, cardY, cardW, cardH);
            AddStatCard(dash, "Inspections",
                GetCount("SafetyInspection", $"InspectionDate >= DATEADD(DAY,-30,GETDATE())"),
                Color.FromArgb(239,68,68),  cardX + 880, cardY, cardW, cardH);

            // Recent rentals grid
            var lbl = new Label
            {
                Text      = "Recent Rental Agreements",
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = TextLight,
                AutoSize  = true,
                Location  = new Point(0, cardY + cardH + 24)
            };
            dash.Controls.Add(lbl);

            var grid = CreateStyledGrid();
            grid.Bounds = new Rectangle(0, cardY + cardH + 55, dash.Width - 10, 300);
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dash.Controls.Add(grid);

            var sql =
                @"SELECT ra.AgreementID AS [ID],
                         e.Model        AS [Equipment],
                         c.CompanyName  AS [Contractor],
                         ra.StartDate   AS [Start Date],
                         ra.EndDate     AS [End Date],
                         ra.ReturnStatus AS [Status]
                  FROM   RentalAgreement ra
                  JOIN   Equipment  e ON e.EquipmentID  = ra.EquipmentID
                  JOIN   Contractor c ON c.ContractorID = ra.ContractorID
                  ORDER BY ra.StartDate DESC";
            grid.DataSource = DatabaseHelper.ExecuteQuery(sql);
        }

        private void AddStatCard(Control parent, string label, string value,
            Color accent, int x, int y, int w, int h)
        {
            var card = new Panel
            {
                Bounds    = new Rectangle(x, y, w, h),
                BackColor = BgCard
            };
            parent.Controls.Add(card);

            var accent2 = new Panel
            {
                Bounds    = new Rectangle(0, 0, 5, h),
                BackColor = accent
            };
            card.Controls.Add(accent2);

            var lbl = new Label
            {
                Text      = label,
                Font      = new Font("Segoe UI", 9f),
                ForeColor = TextMuted,
                AutoSize  = true,
                Location  = new Point(16, 18)
            };
            card.Controls.Add(lbl);

            var val = new Label
            {
                Text      = value,
                Font      = new Font("Segoe UI", 32f, FontStyle.Bold),
                ForeColor = accent,
                AutoSize  = true,
                Location  = new Point(14, 38)
            };
            card.Controls.Add(val);
        }

        private string GetCount(string table, string where = null)
        {
            var sql = $"SELECT COUNT(*) FROM {table}" +
                      (where != null ? $" WHERE {where}" : "");
            var result = DatabaseHelper.ExecuteScalar(sql);
            return result?.ToString() ?? "0";
        }

        // ── Shared helpers ────────────────────────────────────────
        public static DataGridView CreateStyledGrid()
        {
            var grid = new DataGridView
            {
                ReadOnly              = true,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor       = BgCard,
                BorderStyle           = BorderStyle.None,
                RowHeadersVisible     = false,
                GridColor             = Color.FromArgb(50, 80, 100),
                ColumnHeadersHeight   = 38,
                RowTemplate           = { Height = 32 },
                Font                  = new Font("Segoe UI", 9.5f),
                ForeColor             = TextLight
            };
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor  = AccentBlue,
                ForeColor  = Color.White,
                Font       = new Font("Segoe UI", 10f, FontStyle.Bold),
                Alignment  = DataGridViewContentAlignment.MiddleLeft,
                Padding    = new Padding(8, 0, 0, 0)
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor          = BgCard,
                ForeColor          = TextLight,
                SelectionBackColor = Color.FromArgb(59, 130, 246, 60),
                SelectionForeColor = Color.White,
                Padding            = new Padding(6, 0, 0, 0)
            };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor          = Color.FromArgb(38, 52, 72),
                ForeColor          = TextLight,
                SelectionBackColor = Color.FromArgb(59, 130, 246, 60),
                SelectionForeColor = Color.White
            };
            grid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            return grid;
        }

        public static Label MakeLabel(string text, int x, int y,
            int fontSize = 10, bool bold = false, Color? color = null)
        {
            return new Label
            {
                Text     = text,
                Font     = new Font("Segoe UI", fontSize, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color ?? TextLight,
                AutoSize = true,
                Location = new Point(x, y)
            };
        }

        public static TextBox MakeTextBox(int x, int y, int w = 260)
        {
            return new TextBox
            {
                Bounds    = new Rectangle(x, y, w, 32),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = TextLight,
                BorderStyle = BorderStyle.FixedSingle,
                Font      = new Font("Segoe UI", 10f)
            };
        }

        public static ComboBox MakeCombo(int x, int y, int w = 260)
        {
            return new ComboBox
            {
                Bounds      = new Rectangle(x, y, w, 32),
                BackColor   = Color.FromArgb(51, 65, 85),
                ForeColor   = TextLight,
                FlatStyle   = FlatStyle.Flat,
                Font        = new Font("Segoe UI", 10f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
        }

        public static Button MakeButton(string text, int x, int y,
            Color? bg = null, int w = 130, int h = 36)
        {
            var btn = new Button
            {
                Text      = text,
                Bounds    = new Rectangle(x, y, w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg ?? AccentBlue,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
