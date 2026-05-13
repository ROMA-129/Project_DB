using System;
using System.Drawing;
using System.Windows.Forms;

namespace EquipmentRentalApp.Forms
{
    public partial class MainForm : Form
    {
        private Button btnContractors, btnEquipment, btnRentals, btnInspections, btnYards;
        private Panel contentPanel;
        
        public MainForm()
        {
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            this.Text = "Equipment Rental System";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 35);
            
            // Sidebar
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(25, 25, 30)
            };
            
            // Title
            Label lblTitle = new Label
            {
                Text = "📦 Equipment Rental",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(180, 40)
            };
            sidebar.Controls.Add(lblTitle);
            
            // Menu buttons
            int yPos = 80;
            btnContractors = CreateMenuButton("👥 Contractors", yPos);
            btnContractors.Click += (s, e) => LoadForm(new ContractorForm());
            sidebar.Controls.Add(btnContractors);
            
            btnEquipment = CreateMenuButton("🔧 Equipment", yPos += 60);
            btnEquipment.Click += (s, e) => LoadForm(new EquipmentForm());
            sidebar.Controls.Add(btnEquipment);
            
            btnRentals = CreateMenuButton("📋 Rentals", yPos += 60);
            btnRentals.Click += (s, e) => LoadForm(new RentalForm());
            sidebar.Controls.Add(btnRentals);
            
            btnInspections = CreateMenuButton("🔍 Inspections", yPos += 60);
            btnInspections.Click += (s, e) => LoadForm(new InspectionForm());
            sidebar.Controls.Add(btnInspections);
            
            btnYards = CreateMenuButton("🏭 Service Yards", yPos += 60);
            btnYards.Click += (s, e) => LoadForm(new ServiceYardForm());
            sidebar.Controls.Add(btnYards);
            
            // Content panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 50)
            };
            
            this.Controls.Add(contentPanel);
            this.Controls.Add(sidebar);
        }
        
        private Button CreateMenuButton(string text, int y)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(25, 25, 30),
                Size = new Size(200, 45),
                Location = new Point(10, y),
                TextAlign = ContentAlignment.MiddleLeft,
                FlatAppearance = { BorderSize = 0 }
            };
        }
        
        private void LoadForm(Form form)
        {
            contentPanel.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(form);
            form.Show();
        }
    }
}