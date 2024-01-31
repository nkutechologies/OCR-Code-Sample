using Octopus.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Octopus
{
    public partial class Success : BaseForm
    {
        private bool isDragging = false;
        private Point lastCursorPosition;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = Control.MousePosition;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = Control.MousePosition.X - lastCursorPosition.X;
                int deltaY = Control.MousePosition.Y - lastCursorPosition.Y;

                Location = new Point(Location.X + deltaX, Location.Y + deltaY);

                lastCursorPosition = Control.MousePosition;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Success));

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x00010000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public event Action SuccessFormClosed;
        public Success()
        {
            InitializeComponent();
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;

            this.ShowInTaskbar = true; // Ensure it shows in the taskbar
            this.TopMost = true; // Ensure it's displayed above other windows
            this.Activate(); // Ensure the dialog gains focus
            this.BringToFront();
            SetForegroundWindow(this.Handle);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Makes the form non-resizable

            // Subscribe to the Paint event to custom paint the form's background
            //this.Paint += SuccessForm_Paint;
            // Remove the maximize box (button) from the window style
            int currentStyle = GetWindowLong(Handle, GWL_STYLE);
            int newStyle = currentStyle & ~WS_MAXIMIZEBOX;
            SetWindowLong(Handle, GWL_STYLE, newStyle);


            // Set the form border style to None
            this.FormBorderStyle = FormBorderStyle.None;
            // Set padding to create a space around the content
            this.Padding = new Padding(-65); // Adjust the value to your preference

            //// Create PictureBox controls for custom buttons
            //PictureBox minimizeButton = new PictureBox();
            //minimizeButton.Image = FetchIcon("Minimize");
            //minimizeButton.Size = new Size(20, 20);
            //minimizeButton.Location = new Point(this.Width - 42, this.Padding.Top);
            //minimizeButton.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.Click += MinimizeButton_Click;
            //this.Controls.Add(minimizeButton);

            //PictureBox maximizeRestoreButton = new PictureBox();
            //maximizeRestoreButton.Image = FetchIcon("Maximize");
            //maximizeRestoreButton.Size = new Size(30, 30);
            //maximizeRestoreButton.Location = new Point(this.Width - 65, 5);
            //maximizeRestoreButton.SizeMode = PictureBoxSizeMode.StretchImage;
            //maximizeRestoreButton.Click += MaximizeRestoreButton_Click;
            //this.Controls.Add(maximizeRestoreButton);

            //PictureBox closeButton = new PictureBox();
            //closeButton.Image = FetchIcon("Close");
            //closeButton.Size = new Size(20, 20);
            //closeButton.Location = new Point(this.Width - 20, this.Padding.Top);
            //closeButton.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Click += CloseButton_Click;
            //this.Controls.Add(closeButton);

            //minimizeButton.BringToFront();
            //closeButton.BringToFront();
        }


        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //private void MaximizeRestoreButton_Click(object sender, EventArgs e)
        //{
        //    if (isMaximized)
        //    {
        //        this.WindowState = FormWindowState.Normal;
        //        isMaximized = false;
        //    }
        //    else
        //    {
        //        this.WindowState = FormWindowState.Maximized;
        //        isMaximized = true;
        //    }
        //}

        private void CloseButton_Click(object sender, EventArgs e)
        {
            SuccessFormClosed.Invoke();
            this.Close();
        }
        private void SuccessForm_Paint(object sender, PaintEventArgs e)
        {
            // Create a linear gradient brush from top (purple) to bottom (white)
            LinearGradientBrush gradientBrush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.Purple,
                Color.White,
                LinearGradientMode.Vertical); // Vertical gradient

            // Paint the background with the gradient brush
            e.Graphics.FillRectangle(gradientBrush, this.ClientRectangle);
        }
        private void Success_Load(object sender, EventArgs e)
        {

            label3.Font = new Font("Arial", 8, FontStyle.Regular);
            label3.ForeColor = Color.Black;
            label3.BackColor = Color.Transparent;
            label3.TabStop = true;
            label3.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA";

            label1.Font = new Font("Arial", 8, FontStyle.Bold);
            label1.ForeColor = Color.Black;
            label1.BackColor = Color.Transparent;
            label1.TabStop = true;
            label1.Text = "No issues found in discovery validation.";

            label2.Font = new Font("Arial", 9, FontStyle.Bold);
            label2.ForeColor = Color.Black;
            label2.BackColor = Color.Transparent;
            label2.TabStop = true;
            label2.Text = "All Good!";

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        
        private void Success_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        #region close button click handler
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //close button click handler 
            try
            {
                SuccessFormClosed.Invoke();
                this.Hide();
                Dispose();
                //this.Close();
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
            #endregion
        }
    }
}
