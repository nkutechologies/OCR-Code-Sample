using Octopus.Common;
using Octopus.Helpers.CustomHelpers;
using Octopus.Helpers.CustomHelpers.Authentication.Login.Alerts;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Octopus
{
    public partial class Login : BaseForm, IDisposable
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


        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Login));
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //window maximization disabling code starts here
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x00010000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        //private bool isMaximized = false;
        public Login()
        {
            try
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
                pictureBox4.Click += CloseButton_Click;
                //this.Controls.Add(closeButton);

                //minimizeButton.BringToFront();
                //closeButton.BringToFront();
                //pictureBox4.BringToFront();
                UserID.Text = "User Id";
            }
            catch (Exception x)
            {
                _log.Error(x);
            }
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
            this.Close();
        }
        private bool IsValidLogin(string username, string password)
        {
            // Implement your authentication logic here.
            // You should hash and salt the password and compare it to the stored hash coming from db.

            using (var _authenticationService = new AuthenticationService(_apiBaseURl, username, password))
            {
                var authToken = _authenticationService.GetAuthToken();

                if (authToken.Equals(Alert.LoginFailedMessage))
                {
                    return false;
                }
                else
                {
                    // Save auth token to app settings for later use
                    AppSettingManager.Save("authToken", authToken);
                    return true;
                }
            }
        }

        #region login submit click handler code goes here
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //Login Submit click handler
            try
            {
                pictureBox2.Visible = false;
                pictureBox3.Visible = true;
                if (!string.IsNullOrEmpty(UserID.Text) && !string.IsNullOrEmpty(Password.Text))
                {
                    if (IsValidLogin(UserID.Text, Password.Text))
                    {
                        pictureBox2.Visible = true;
                        pictureBox3.Visible = false;

                        // Check if the "Remember Me" checkbox is checked.
                        bool rememberMe = chkRememberMe.Checked;

                        // Save the username if "Remember Me" is selected.
                        if (rememberMe)
                        {
                            AppSettingManager.Save("appUserName", UserID.Text);
                            AppSettingManager.Save("appPassword", Password.Text);
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            AppSettingManager.Reset("appUserName");
                            AppSettingManager.Reset("appPassword");
                        }
                        this.Hide();
                        this.notifyIcon1.Visible = false;
                        myProfile myProfile = new myProfile();
                        myProfile.Show();
                    }
                    else
                    {
                        MessageBox.Show(Alert.LoginFailedMessage);
                        pictureBox2.Visible = true;
                        pictureBox3.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show(Alert.UsernamePasswordIsRequired);
                    pictureBox2.Visible = true;
                    pictureBox3.Visible = false;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        #endregion 

        private void Login_Load(object sender, EventArgs e)
        {
            // Assuming textboxUsername and textboxPassword are TextBox controls
            UserID.Font = new Font("Segoe UI", 15f, FontStyle.Regular);
            Password.Font = new Font("Segoe UI", 15f, FontStyle.Regular);
            //label3.Font = new Font("Arial", 8, FontStyle.Bold);
            ////label3.Font = new Font("Montserrat", 8, FontStyle.Regular);
            //// Set the label's ForeColor property to the created color
            //label3.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));
            //label3.TabStop = true;
            materialLabel1.Font= new Font("Montserrat", 9, FontStyle.Regular);
            materialLabel1.BackColor = Color.Transparent;
            materialLabel1.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA";

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            ConvertResolution();
            //reset auth token with empty string
            AppSettingManager.Reset("authToken");
            if (!string.IsNullOrEmpty(_authUserId) && !string.IsNullOrEmpty(_authLoginPassword))
            {
                UserID.Text = _authUserId;
                Password.Text = _authLoginPassword;
                chkRememberMe.Checked = true;
            }
            //label3.Font = new Font("Montserrat", 8, FontStyle.Regular); // Assuming "Montserrat" is available in your system fonts
            //label3.ForeColor = ColorTranslator.FromHtml("#392b65");
            //label3.Font = new Font(label3.Font, FontStyle.Regular); // Set font weight to regular (400)

            var sizeOfScreen = CurrentDisplayResolution;

            // Check the screen resolution and apply adjustments accordingly
            if (sizeOfScreen.Width <= 1400 && sizeOfScreen.Height <= 1050)
            {
                pictureBox1.Width = this.ClientSize.Width + 10;
                pictureBox1.Height = this.ClientSize.Height + 4;
                pictureBox5.Location = new Point(pictureBox5.Location.X-32, pictureBox5.Location.Y);
                pictureBox4.Location = new Point(pictureBox4.Location.X - 32, pictureBox4.Location.Y);
            }

        }

        private void Login_Resize(object sender, EventArgs e)
        {
            try
            {
                // Check if notifyIcon1 is not null before accessing it
                if (this.WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    notifyIcon1.Visible = true;
                    if (_appSettings.Configurations.IsNotificationEnabled)
                    {
                        notifyIcon1.ShowBalloonTip(500);
                    }
                }
            }
            catch (Exception x)
            {
                _log.Error(x);
            }
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void Login_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }
    }
}