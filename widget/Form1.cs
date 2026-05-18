using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace widget
{
    public partial class Form1 : Form
    {
        // WIN32 APIs
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private const int GWL_HWNDPARENT = -8;
        // --------------------------------

        public Form1()
        {
            InitializeComponent();

            label1.UseMnemonic = false;
            label2.UseMnemonic = false;
            
            // -------------------------------------------------------------------

            this.ShowInTaskbar = false;
            this.Size = new Size(1300, 1000);
            this.StartPosition = FormStartPosition.Manual;

            label2.AutoSize = false;
            label2.Size = new Size(1000, 180); // 160px height
            label2.TextAlign = ContentAlignment.TopCenter; // Anchored to top!

            label1.AutoSize = false;
            label1.Size = new Size(930, 105); // 70px height
            label1.TextAlign = ContentAlignment.TopCenter;

            // -----------------------------------------------------

            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int xPosition = (screenWidth - this.Width) / 2;
            int yPosition = (int)(screenHeight * 0.20);
            this.Location = new Point(xPosition, yPosition);

            timer1.Tick += Timer1_Tick;
            UpdateClock();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IntPtr desktopHandle = FindWindow("Progman", null);

            if (desktopHandle != IntPtr.Zero)
            {
                if (IntPtr.Size == 8)
                    SetWindowLongPtr64(this.Handle, GWL_HWNDPARENT, desktopHandle);
                else
                    SetWindowLong32(this.Handle, GWL_HWNDPARENT, desktopHandle.ToInt32());
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x800A0;
                return cp;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            string rawTime = DateTime.Now.ToString("HH : mm : ss");
            label2.Text = TranslateNumbersToAakriti(rawTime);

            // Process Day
            string rawDay = DateTime.Now.ToString("dddd");
            string finalDay = TranslateDayToAakriti(rawDay);

            // Process Date
            string rawDate = DateTime.Now.ToString("dd . MM");
            string finalDate = TranslateNumbersToAakriti(rawDate);

            // Join
            label1.Text = finalDay + "\u00A0\u00A0\u00A0\u00A0\u00A0" + finalDate;

            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;

            label2.Top = 20;               // Time
            label1.Top = 200;              // Day + Date combined line

            label1.BringToFront();

        }

        private string TranslateNumbersToAakriti(string standardTimeOrDate)
        {
            var charMap = new Dictionary<char, char>
            {
                { '0', ')' }, { '1', '!' }, { '2', '@' }, { '3', '#' }, { '4', '$' },
                { '5', '%' }, { '6', '^' }, { '7', '&' }, { '8', '*' },
                { '9', '(' }, { ':', '：' }, { '.', '.' }
            };

            string translated = "";
            foreach (char c in standardTimeOrDate)
            {
                if (charMap.ContainsKey(c))
                    translated += charMap[c];
                else
                    translated += c;
            }
            return translated;
        }

        private string TranslateDayToAakriti(string englishDay)
        {
            var dayMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Monday",    ";f]djf/" },
                { "Tuesday",   "d+unjf/" },
                { "Wednesday", "a'wjf/" },
                { "Thursday",  "u'?jf/" },
                { "Friday",    "z's|jf/" },
                { "Saturday",  "zlgjf/" },
                { "Sunday",    "/ljjf/" }
            };

            if (dayMap.ContainsKey(englishDay))
                return dayMap[englishDay];

            return englishDay;
        }

        private void label2_Click(object sender, EventArgs e) { }
    }
}
