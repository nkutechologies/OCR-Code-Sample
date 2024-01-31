using Octopus.Common;
using Octopus.Dtos.CPTSuggestionDtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Octopus
{
    public partial class CPTSuggestions : BaseForm
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






        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x00010000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(CPTSuggestions));
        private int selectedCheckboxCount = 0;
        private List<CPTSuggestionsResponseDto> _suggestionsResponseDtos = new List<CPTSuggestionsResponseDto>();
        public CPTSuggestions(List<CPTSuggestionsResponseDto> cPTSuggestionsResponseDto)
        {
            try
            {
                _suggestionsResponseDtos = cPTSuggestionsResponseDto;
                InitializeComponent();
                pictureBox1.MouseDown += pictureBox1_MouseDown;
                pictureBox1.MouseMove += pictureBox1_MouseMove;
                pictureBox1.MouseUp += pictureBox1_MouseUp;

                label2.Text = $"(0) out of {cPTSuggestionsResponseDto.Sum(x => x.items.Count)} is been selected";
                CreateDynamicControls(cPTSuggestionsResponseDto);
                this.TopMost = true; // Ensure it's displayed above other windows
                this.Activate(); // Ensure the dialog gains focus
                this.BringToFront();
                SetForegroundWindow(this.Handle);
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private string GetIconPath(string serviceType)
        {
            if (serviceType == "Laboratory")
            {
                return "lab1.png";
            }
            else
            {
                return "icons8-radiation-50.png";
            }
        }
        private List<string> GetSelectedCheckBoxesText()
        {
            return panel1.Controls.OfType<CheckBox>()
                                  .Where(checkBox => checkBox.Checked)
                                  .Select(checkBox => checkBox.Text)
                                  .ToList();
        }
        private void CreateDynamicControls(List<CPTSuggestionsResponseDto> cPTSuggestionsResponseDto)
        {
            int yOffset = 23; // Initial Y position

            foreach (var service in cPTSuggestionsResponseDto.Where(x => x.serviceType != null))
            {
                // Create the icon PictureBox based on the service type
                string iconPath = GetIconPath(service.serviceType);
                // Load the icon image based on the iteration
                string currentDirectory = CurrentWorkingDirectory;
                string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", iconPath);
                var iConimage = _helperService.LoadImage(iconImagePath);

                var iconPictureBox = new PictureBox
                {
                    Location = new System.Drawing.Point(29, yOffset),
                    Size = new System.Drawing.Size(15, 16), // Adjust the size as needed
                    Image = iConimage,//_helperService.LoadImage(iconImagePath),//Image.FromFile(iconImagePath),
                    SizeMode = PictureBoxSizeMode.Zoom,
                };

                panel1.Controls.Add(iconPictureBox);

                // Create the label based on the service type
                var label = new Label
                {
                    Location = new System.Drawing.Point(44, yOffset), // Adjust the X position to leave space for the icon
                    Text = service.serviceType,
                    Size = new System.Drawing.Size(100, 20), // Adjust the size as needed
                    Font = new System.Drawing.Font("Arial Black", 8, FontStyle.Regular), // Set the font to bold
                };
                panel1.Controls.Add(label);

                yOffset += Math.Max(iconPictureBox.Height, label.Height) + 5; // Adjust Y position for the next controls

                foreach (var item in service.items)
                {
                    // Create the checkbox for the item
                    var labCheckBox = new CheckBox
                    {
                        Location = new System.Drawing.Point(30, yOffset), // Adjust the X position to align with the label
                        Text = $"{item.code}                    {item.description}",
                        Size = new System.Drawing.Size(400, 20), // Adjust the size as needed
                        Font = new System.Drawing.Font(Font.FontFamily, 7, FontStyle.Regular), // Set the font to regular
                    };
                    panel1.Controls.Add(labCheckBox);

                    yOffset += labCheckBox.Height + 5; // Adjust Y position for the next controls

                    labCheckBox.CheckedChanged += (sender, e) =>
                    {
                        // When a checkbox is checked or unchecked, update the selectedCheckboxCount
                        if (labCheckBox.Checked)
                        {
                            selectedCheckboxCount++;
                        }
                        else
                        {
                            selectedCheckboxCount--;
                        }

                        // Update the label text to reflect the new count
                        label2.Text = $"({selectedCheckboxCount}) out of {cPTSuggestionsResponseDto.Sum(x => x.items.Count)} is been selected";
                    };
                }

                // Create the separator below all the checkboxes for the current service type
                var separator = new Label
                {
                    Location = new System.Drawing.Point(30, yOffset), // Adjust the X position to align with the label
                    Text = "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------",
                    Font = new System.Drawing.Font(Font.FontFamily, 2, FontStyle.Bold), // Set the font to bold
                    Size = new System.Drawing.Size(620, 10), // Adjust the size as needed
                };
                panel1.Controls.Add(separator);

                yOffset += separator.Height + 5; // Adjust Y position for the next service type

            }
        }

        private void CPTSuggestions_Load(object sender, EventArgs e)
        {

            // Remove the title bar and border
            FormBorderStyle = FormBorderStyle.None;

            // Hide the minimize, maximize, and close buttons
            ControlBox = false;

            //label1.Font = new Font("Arial", 9, FontStyle.Regular);
            // Set the ForeColor property of the label
            label1.Font = new Font("Montserrat", 8, FontStyle.Regular); // Assuming "Montserrat" is available in your system fonts
            label1.ForeColor = ColorTranslator.FromHtml("#392b65");
            label1.Font = new Font(label1.Font, FontStyle.Bold); // Set font weight to bold (600)
            label1.BringToFront();

            label3.Font = new Font("Montserrat", 8, FontStyle.Regular);
            // Set the label's ForeColor property to the created color
            label3.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));
            label3.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA.";

            label2.Font = new Font("Montserrat", 8, FontStyle.Regular);
            //label6.ForeColor = ColorTranslator.FromHtml("#392b65");
            //label6.ForeColor = Color.Black;
            label2.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));
            label2.BackColor = Color.Transparent;
            label2.Paint += (senderr, ee) => { };

            label5.Text = "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
            label5.AutoSize = false;
            label5.Size = new Size(panel1.ClientSize.Width + 24, 1); // Set the width to match the container's width
            label5.Margin = new Padding(10, 10, 0, 10); // Margin similar to the CSS margin
            label5.BorderStyle = BorderStyle.None; // Remove border

            // Apply a gradient-like background color
            label5.BackColor = ColorTranslator.FromHtml("#49a3f1"); // Linear gradient from #49a3f1 to #1a73e8

            // Apply a border-like appearance using a lower opacity color
            label5.Paint += (senderr, ee) =>
            {
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#49a3f1"), 0.2f)) // Opacity: .25
                {
                    ee.Graphics.DrawLine(pen, new Point(0, label5.Height - 1), new Point(label5.Width, label5.Height - 1));
                }
            };
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();

            Dispose();
            //this.Close();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.Hide();

            Dispose();
            //this.Close();
        }

        #region accept button click handler code goes here
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //accept button click handler code goes here

            //hide this form
            this.Hide();

            MouseScrollUp();

            //fetch selectedCheckboxes text from suggestion winform dialogue so that we can pass it in the payload of octupus api in order to fetch icdcodes using endpoint "/api/Octopus/GetICDSuggestion"
            List<string> selectedServices = GetSelectedCheckBoxesText();
            if (selectedServices.Count() == 0)
            {

                Dispose();
                //this.Close();
                return;
            }

            //show confirm box for asking Are all the order tab sections are expanded? in order to collapse
            CollapseExpandedOrderSectionsIfNeeded();

             // Extract codes from selectedServices and create a new list with key-value pairs
             var codeTypePairs = selectedServices
                .Select(item =>
                {
                    var match = Regex.Match(item, @"\d+"); // Extract code from selectedService item
                    if (match.Success)
                    {
                        var code = match.Value;

                        // Find the item that matches the code
                        var matchedItem = _suggestionsResponseDtos
                                        .SelectMany(dto => dto.items) // Flatten the list of items from all CPTSuggestionsResponseDto
                                        .FirstOrDefault(dtoItem => dtoItem.code == code);

                        if (matchedItem != null)
                        {
                            // Retrieve the type from the matched item
                            var itemType = matchedItem.custHISServiceType;
                            return new KeyValuePair<string, string>(itemType, code); // Create a key-value pair with custHISServiceType as key and code as value
                        }
                    }
                    return default(KeyValuePair<string, string>); // If no match found, return default KeyValuePair
                })
                .Where(pair => !pair.Equals(default(KeyValuePair<string, string>))) // Remove default KeyValuePair items
                .ToList();

            // Now 'codeTypePairs' contains the key-value pairs of custHISServiceType and code extracted from selectedServices strings
            if (codeTypePairs.Any())
            {
                var labortoryCPTS = codeTypePairs.Where(x => x.Key.Equals("Laboratory"));
                var radiologyCPTS = codeTypePairs.Where(x => x.Key.Equals("Radiology"));
                if (labortoryCPTS.Any())
                {
                    AddAndDeleteCPTInLabOrder(labortoryCPTS.Select(x=>x.Value).ToList(), removeRowCount:0, deletionRequired:false);
                }
                if (radiologyCPTS.Any())
                {
                    AddAndDeleteCPTInRadiology(radiologyCPTS.Select(x=>x.Value).ToList(), removeRowCount:0, deletionRequired:false);
                }
            }
        }
        #endregion accept button click handler code goes here

        private void CPTSuggestions_Resize(object sender, EventArgs e)
        {
            //pictureBox1.Width = this.ClientSize.Width + 10;
            //pictureBox1.Height = this.ClientSize.Height + 4;
        }
    }
}
