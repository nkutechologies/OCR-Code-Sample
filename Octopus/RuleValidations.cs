using Octopus.Common;
using Octopus.Dtos.ValidateRulesDtos;
using Octopus.Models.Singleton.DignosisGridDtos;
using Octopus.Models.Singleton.ServicesGridDtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Octopus
{
    public partial class RuleValidations : BaseForm, IDisposable
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


        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(RuleValidations));

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

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
        private int _errorCount { get; }
        List<RulesValidationResult> _errorSolutions = new List<RulesValidationResult>();
        public event Action RuleValidationOperationFinished;

        public RuleValidations(List<RulesValidationResult> errorSolutions)
        {
            try
            {
                InitializeComponent();
                pictureBox2.MouseDown += pictureBox1_MouseDown;
                pictureBox2.MouseMove += pictureBox1_MouseMove;
                pictureBox2.MouseUp += pictureBox1_MouseUp;

                _errorSolutions = errorSolutions;
                _errorCount = errorSolutions.Count();

                int resolvedErrorCount = 0; // Counter for resolved errors
                // Disable resizing and maximize button
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;

                // Set the form's start position to center screen
                this.StartPosition = FormStartPosition.CenterScreen;

                int startY = label1.Bottom - 15; // Start Y position below label1 with more space

                // Create the icon PictureBox based on the service type
                string iconPath = "daner-05.png";

                // Load the icon image based on the iteration
                string currentDirectory = CurrentWorkingDirectory;

                string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", iconPath);
                Image iconImage = _helperService.LoadImage(iconImagePath);

                int iconSpacing = 10; // Adjust the spacing between the icon and checkbox
                int checkboxSpacing = 10; // Adjust the spacing between the checkbox label and checkbox

                foreach (var response in errorSolutions)
                {
                    // Create a PictureBox for the image
                    var errorImage = new PictureBox
                    {
                        Location = new Point(10, startY - 3),
                        Size = new Size(20, 23), // Set the size of the image as needed
                        Image = iconImage,//_helperService.LoadImage(iconImagePath),//Image.FromFile(iconImagePath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                    };

                    // Create a label for the error message
                    var errorLabel = new Label
                    {
                        Text = response.errorMessage,
                        Location = new Point(errorImage.Right - 10 + iconSpacing, startY),
                        AutoSize = true, // Adjusts label size to content
                        ForeColor = Color.Black,
                        Font = new Font("Arial", 12, FontStyle.Regular)
                    };

                    // Create a checkbox for the solution
                    var solutionCheckBox = new CheckBox
                    {
                        Text = response.solution,
                        Location = new Point(errorImage.Left, errorLabel.Bottom + checkboxSpacing), // Adjust spacing as needed
                        AutoSize = true // Adjusts checkbox size to content
                    };

                    // Apply font to the CheckBox's label text
                    solutionCheckBox.Font = new Font("Segoe UI", 10f, FontStyle.Regular);

                    // Attach an event handler for the CheckedChanged event
                    solutionCheckBox.CheckedChanged += (sender, e) =>
                    {
                        if (solutionCheckBox.Checked)
                        {
                            resolvedErrorCount++;
                        }
                        else
                        {
                            resolvedErrorCount--;
                        }

                        // Update label2.Text to show the count of resolved errors
                        label2.Text = $"{resolvedErrorCount} Errors resolved out of {_errorCount}";
                    };

                    // Create a tooltip for the checkbox
                    ToolTip toolTip = new ToolTip();
                    toolTip.SetToolTip(solutionCheckBox, response.itemDescription);

                    // Add the controls to the panel1
                    panel1.Controls.Add(errorImage);
                    panel1.Controls.Add(errorLabel);
                    panel1.Controls.Add(solutionCheckBox);

                    // Update the starting Y position for the next controls
                    startY += Math.Max(errorImage.Height, errorLabel.Height + solutionCheckBox.Height) + 30; // Adjust spacing as needed
                }
                this.TopMost = true; // Ensure it's displayed above other windows
                this.Activate(); // Ensure the dialog gains focus
                this.BringToFront();
                SetForegroundWindow(this.Handle);
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        //public RuleValidations(List<RulesValidationResult> errorSolutions)
        //{
        //    try
        //    {
        //        InitializeComponent();
        //        pictureBox2.MouseDown += pictureBox1_MouseDown;
        //        pictureBox2.MouseMove += pictureBox1_MouseMove;
        //        pictureBox2.MouseUp += pictureBox1_MouseUp;

        //        _errorSolutions = errorSolutions;
        //        _errorCount = errorSolutions.Count();
        //        int resolvedErrorCount = 0; // Counter for resolved errors
        //        bool anyCheckboxChecked = false; // Flag to track if any checkbox is checked

        //        // Disable resizing and maximize button
        //        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        //        this.MaximizeBox = false;

        //        // Set the form's start position to center screen
        //        this.StartPosition = FormStartPosition.CenterScreen;

        //        int startY = label1.Bottom - 15; // Start Y position below label1 with more space

        //        // Create the icon PictureBox based on the service type
        //        string iconPath = "daner-05.png";

        //        // Load the icon image based on the iteration
        //        string currentDirectory = CurrentWorkingDirectory;

        //        string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", iconPath);
        //        Image iconImage = _helperService.LoadImage(iconImagePath);

        //        int iconSpacing = 10; // Adjust the spacing between the icon and checkbox
        //        int checkboxSpacing = 10; // Adjust the spacing between the checkbox label and checkbox

        //        foreach (var response in errorSolutions)
        //        {
        //            // Create a PictureBox for the image
        //            var errorImage = new PictureBox
        //            {
        //                Location = new Point(10, startY - 3),
        //                Size = new Size(20, 23), // Set the size of the image as needed
        //                Image = iconImage,
        //                SizeMode = PictureBoxSizeMode.Zoom,
        //            };

        //            // Create a label for the error message
        //            var errorLabel = new Label
        //            {
        //                Text = response.errorMessage,
        //                Location = new Point(errorImage.Right - 10 + iconSpacing, startY),
        //                AutoSize = true,
        //                ForeColor = Color.Black,
        //                Font = new Font("Arial", 12, FontStyle.Regular)
        //            };

        //            // Create a CheckBox for the solution
        //            var solutionCheckBox = new CheckBox
        //            {
        //                Text = response.solution,
        //                Location = new Point(errorImage.Left, errorLabel.Bottom + checkboxSpacing),
        //                AutoSize = true
        //            };

        //            // Attach an event handler for the CheckedChanged event
        //            solutionCheckBox.CheckedChanged += (sender, e) =>
        //            {
        //                if (solutionCheckBox.Checked)
        //                {
        //                    resolvedErrorCount++;
        //                }
        //                else
        //                {
        //                    resolvedErrorCount--;
        //                }

        //                // Check if any checkbox is checked after the change
        //                anyCheckboxChecked = panel1.Controls.OfType<CheckBox>().Any(checkbox => checkbox.Checked);

        //                // Update label2.Text to show the count of resolved errors
        //                label2.Text = $"{resolvedErrorCount} Errors resolved out of {_errorCount}";

        //                // Update the icon based on the checkbox state
        //                string newIconPath = anyCheckboxChecked ? "WhatsApp Image 2024-01-18 at 11.05.05 AM.jpeg" : "daner-05.png";
        //                string newIconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", newIconPath);
        //                errorImage.Image = _helperService.LoadImage(newIconImagePath);

        //                Application.DoEvents();
        //                // Refresh the panel to force a visual update
        //                panel1.Invalidate();
        //            };


        //            // Apply font to the CheckBox's label text
        //            solutionCheckBox.Font = new Font("Segoe UI", 10f, FontStyle.Regular);

        //            // Add the controls to panel1
        //            panel1.Controls.Add(errorImage);
        //            panel1.Controls.Add(errorLabel);
        //            panel1.Controls.Add(solutionCheckBox);

        //            // Update the starting Y position for the next controls
        //            startY += Math.Max(errorImage.Height, errorLabel.Height + solutionCheckBox.Height) + 30;
        //        }

        //        this.TopMost = true;
        //        this.Activate();
        //        this.BringToFront();
        //        SetForegroundWindow(this.Handle);
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}
        private List<string> GetSelectedCheckBoxesText()
        {
            return panel1.Controls.OfType<CheckBox>()
                                  .Where(checkBox => checkBox.Checked)
                                  .Select(checkBox => checkBox.Text)
                                  .ToList();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposed)
        //    {
        //        if (disposing)
        //        {
        //            //// Dispose managed resources here
        //            //if (_HelperService != null && _HelperService is IDisposable disposable)
        //            //{
        //            //    disposable.Dispose();
        //            //}
        //        }

        //        // Dispose unmanaged resources here (if any)

        //        disposed = true;
        //    }
        //}
        ~RuleValidations()
        {
            Dispose(false);
        }

        private void RuleValidations_Load(object sender, EventArgs e)
        {

            // Remove the title bar and border
            FormBorderStyle = FormBorderStyle.None;

            // Hide the minimize, maximize, and close buttons

            // Set the text for label2 to display the error list count
            label2.Text = $"0 Errors resolved out of {_errorCount}";

            label3.Font = new Font("Montserrat", 8, FontStyle.Regular);
            label3.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));

            //label5.Font = new Font("Arial", 8, FontStyle.Regular);
            //// Set the label's ForeColor property to the created color
            //label5.ForeColor = ColorTranslator.FromHtml("#230A7F");

            label2.Font = new Font("Arial", 8, FontStyle.Regular);
            // Set the label's ForeColor property to the created color
            label2.ForeColor = ColorTranslator.FromHtml("#230A7F");
            label3.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA.";

            label4.Text = "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
            label4.AutoSize = false;
            label4.Size = new Size(panel1.ClientSize.Width + 24, 1); // Set the width to match the container's width
            label4.Margin = new Padding(10, 10, 0, 10); // Margin similar to the CSS margin
            label4.BorderStyle = BorderStyle.None; // Remove border

            // Apply a gradient-like background color
            label4.BackColor = ColorTranslator.FromHtml("#49a3f1"); // Linear gradient from #49a3f1 to #1a73e8

            // Apply a border-like appearance using a lower opacity color
            label4.Paint += (senderr, ee) =>
            {
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#49a3f1"), 0.2f)) // Opacity: .25
                {
                    ee.Graphics.DrawLine(pen, new Point(0, label5.Height - 1), new Point(label5.Width, label5.Height - 1));
                }
            };


            label2.Font = new Font("Montserrat", 8, FontStyle.Regular);
            //label6.ForeColor = ColorTranslator.FromHtml("#392b65");
            //label6.ForeColor = Color.Black;
            label2.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));
            label2.BackColor = Color.Transparent;
            label2.Paint += (senderr, ee) => { };

            // Set the ForeColor property of the label
            label5.Font = new Font("Montserrat", 8, FontStyle.Regular); // Assuming "Montserrat" is available in your system fonts
            label5.ForeColor = ColorTranslator.FromHtml("#392b65");
            label5.Font = new Font(label1.Font, FontStyle.Bold); // Set font weight to bold (600)

            var sizeOfScreen = CurrentDisplayResolution;

            // Check the screen resolution and apply adjustments accordingly
            if (sizeOfScreen.Width <= 1400 && sizeOfScreen.Height <= 1050)
            {
                this.Size = new Size(923, 499);
            }

        }

        #region reject button click handler code goes here
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            try
            {
                RuleValidationOperationFinished.Invoke();
                this.Hide();
                Dispose();
                //this.Close();
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
                //this.Close();
            }
        }
        #endregion reject button click handler code goes here

        #region accept button click handler code goes here
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            //accept button click handler code goes here
            try
            {
                this.Hide();
                MouseScrollUp();
                var errors = GetSelectedCheckBoxesText();
                if (errors.Count() == 0)
                {

                    Dispose();
                    //this.Close();
                    RuleValidationOperationFinished.Invoke();
                    return;
                }

                //show confirm box for asking Are all the order tab sections are expanded? in order to collapse
                CollapseExpandedOrderSectionsIfNeeded();

                var solutions = errors
                .Select(i =>
                {
                    var err = _errorSolutions.FirstOrDefault(x => x.solution.Equals(i));
                    return err;
                })
               .Where(res => res != null)
               .ToList();

                var errSolutions = solutions;

                #region retrieving singleton dignosis list

                //make the instance of our custom singelton list
                DiagnosisGridDtoList diagnosisGridDtoList = DiagnosisGridDtoList.GetInstance();

                // 3. Retrieving the list of DiagnosisGridDto objects from the shared instance
                List<DiagnosisGridDto> diagnosisGridDtos = diagnosisGridDtoList.GetDiagnosisList();

                #endregion retrieving singleton dignosis list

                #region retrieving services list singleton list

                ServiceGridDtoList serviceGridDtoList = ServiceGridDtoList.GetInstance();

                // Retrieving the list of DiagnosisGridDto objects from the shared instance
                List<ServicesGridDto> servicesGridDtos = serviceGridDtoList.GetServiceList();

                #endregion retrieving services list singleton list

                if (errSolutions.Any())
                {
                    var dxinfoesDel = errSolutions.Where(x => x.Section.Equals("DxInfos") && x.action.Equals("Remove")).ToList();
                    var dxinfoesAdd = errSolutions.Where(x => x.Section.Equals("DxInfos") && x.action.Equals("Add")).ToList();

                    var servicesDel = errSolutions.Where(x => x.Section.Equals("Services") && x.action.Equals("Remove")).ToList();
                    var servicesAdd = errSolutions.Where(x => x.Section.Equals("Services") && x.action.Equals("Add")).ToList();

                    var servicesReplace = errSolutions.Where(x => x.Section.Equals("Services") && x.action.Equals("Replace")).ToList();

                    //icd deletion case
                    if (dxinfoesDel.Any())
                    {
                        int beforeRemoveItemCount = diagnosisGridDtos.Count()+1;
                        var codes = dxinfoesDel.Select(x => x.item);

                        if (codes.Any())
                        {
                            var primaryCodeInDel = diagnosisGridDtos.FirstOrDefault(y => y.Type == "Primary" && codes.Contains(y.Code))?.Code;

                            var newPrimaryCode = dxinfoesDel.FirstOrDefault(x => x.item == primaryCodeInDel)?
                                .errorMessage?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)?
                                .SkipWhile(s => !s.ToLower().Equals("with"))?
                                .Skip(1)?
                                .FirstOrDefault();

                            diagnosisGridDtoList.RemoveByCodes(codes);

                            if (!string.IsNullOrEmpty(newPrimaryCode))
                            {
                                List<DiagnosisGridDto> newData = codes.Select(i => new DiagnosisGridDto { Code = newPrimaryCode, Type = "Primary" }).ToList();
                                diagnosisGridDtoList.AddDiagnosisList(newData);
                            }

                            var allCodes = diagnosisGridDtos
                                .OrderByDescending(x => x.Type == "Primary")
                                .Select(x => x.Code)
                                .Distinct()
                                .ToList();

                            AddAndDeleteICDinDignosisTab(allCodes, beforeRemoveItemCount, deletionRequired: true, primaryAdditionRequired: true);
                        }
                    }

                    //icd addition case
                    if (dxinfoesAdd.Any())
                    {
                        //int beforeAdditionItemCount = diagnosisGridDtos.Count();

                        var codes = dxinfoesAdd.Select(x => x.item);

                        if (codes.Any())
                        {
                            // Assuming 'dxinfoesAdd' is a collection you're iterating through
                            //List<DiagnosisGridDto> newData = codes.Select(i => new DiagnosisGridDto { Code = i,Type="Secondary" }).ToList();

                            // Add the new data to the existing list in the singleton instance
                            //diagnosisGridDtoList.AddDiagnosisList(newData);

                            AddAndDeleteICDinDignosisTab(codes.ToList(), removeRowCount: 0, deletionRequired: false, primaryAdditionRequired: false);
                        }
                    }

                    //cpt deletion case
                    if (servicesDel.Any())
                    {
                        int beforeDeletionRadiologyItemCount = servicesGridDtos.Where(x => x.Type == "Radiology").Count();
                        int beforeDeletionLaboratoryItemCount = servicesGridDtos.Where(x => x.Type == "Laboratory").Count();

                        ////Radiology case
                        var serviceDelItemRemovalRadiologyCodes = servicesDel
                        .Where(x => x != null && x.CustHISServiceType != null && x.CustHISServiceType.Equals("Radiology"))
                        .SelectMany(x => x.ItemRemoval.Where(i => i != null && i.ClientServiceType != null && i.ClientServiceType.Equals("Radiology")))
                        .Select(i => i?.Code) // Adding a null-conditional operator to handle the possibility of 'i' being null
                        .Where(code => code != null) // Filtering out null codes, if any
                        .ToList();

                        if (serviceDelItemRemovalRadiologyCodes.Any())
                        {
                            serviceGridDtoList.RemoveByCodes(serviceDelItemRemovalRadiologyCodes);

                            AddAndDeleteCPTInRadiology(servicesGridDtos.Where(x => x.Type == "Radiology").Select(x => x.Code).ToList(), beforeDeletionRadiologyItemCount, deletionRequired: true);

                        }

                        //// labortory case 
                        var serviceDelItemRemovalLabortoryCodes = servicesDel
                          .Where(x => x != null && x.CustHISServiceType != null && x.CustHISServiceType.Equals("Laboratory"))
                         .SelectMany(x => x.ItemRemoval.Where(i => i != null && i.ClientServiceType != null && i.ClientServiceType.Equals("Laboratory")))
                         .Select(i => i?.Code)
                         .ToList();

                        if (serviceDelItemRemovalLabortoryCodes.Any())
                        {
                            serviceGridDtoList.RemoveByCodes(serviceDelItemRemovalLabortoryCodes);

                            AddAndDeleteCPTInLabOrder(servicesGridDtos.Where(x => x.Type == "Laboratory").Select(x => x.Code).ToList(), beforeDeletionLaboratoryItemCount, deletionRequired: true);
                        }

                    }

                    //cpt addition case
                    if (servicesAdd.Any())
                    {
                        //int beforeDeletionRadiologyItemCount = servicesGridDtos.Where(x => x.Type == "Radiology").Count();
                        //int beforeDeletionLaboratoryItemCount = servicesGridDtos.Where(x => x.Type == "Laboratory").Count();

                        // // Radiology case
                        var serviceDelItemAdditionRadiologyCodes = servicesAdd
                        .Where(x => x != null && x.CustHISServiceType != null && x.CustHISServiceType.Equals("Radiology"))
                        .SelectMany(x => x.ItemAddition.Where(i => i != null && i.ClientServiceType != null && i.ClientServiceType.Equals("Radiology")))
                        .Select(i => i?.Code)
                        .Where(code => code != null)
                        .ToList();


                        if (serviceDelItemAdditionRadiologyCodes.Any())
                        {
                            // Assuming 'dxinfoesAdd' is a collection you're iterating through
                            //List<ServicesGridDto> newData = serviceDelItemAdditionRadiologyCodes.Select(i => new ServicesGridDto { Code = i,Type= "Radiology" }).ToList();

                            // Add the new data to the existing list in the singleton instance
                            //serviceGridDtoList.AddServiceList(newData);

                            AddAndDeleteCPTInRadiology(serviceDelItemAdditionRadiologyCodes.ToList(), removeRowCount: 0, deletionRequired: false);
                        }

                        //Laboratory case
                        var serviceAddItemAdditionLabortaryCodes = servicesAdd
                       .Where(x => x != null && x.CustHISServiceType != null && x.CustHISServiceType.Equals("Laboratory"))
                       .SelectMany(x => x.ItemAddition.Where(i => i != null && i.ClientServiceType != null && i.ClientServiceType.Equals("Laboratory")))
                       .Select(i => i?.Code)
                       .Where(code => code != null)
                       .ToList();


                        if (serviceAddItemAdditionLabortaryCodes.Any())
                        {
                            // Assuming 'dxinfoesAdd' is a collection you're iterating through
                            //List<ServicesGridDto> newData = serviceAddItemAdditionLabortaryCodes.Select(i => new ServicesGridDto { Code = i,Type= "Laboratory" }).ToList();

                            // Add the new data to the existing list in the singleton instance
                            //serviceGridDtoList.AddServiceList(newData);

                            AddAndDeleteCPTInLabOrder(serviceAddItemAdditionLabortaryCodes.ToList(), removeRowCount: 0, deletionRequired: false);
                        }
                    }

                    //cpt replace case
                    if (servicesReplace.Any())
                    {
                        int beforeDeletionLaboratoryItemCount = servicesGridDtos.Where(x => x.Type == "Laboratory").Count();
                        int beforeDeletionRadiologyItemCount = servicesGridDtos.Where(x => x.Type == "Radiology").Count();

                        ////Radiology case
                        var serviceDelItemRemovalRadiologyCodes = servicesReplace
                        .Where(x => x.ReplaceCusHISServiceType != null && x.ReplaceCusHISServiceType.Equals("Investigation"))
                        .SelectMany(x => x.ItemRemoval)
                        .Where(i => i != null && i.ClientServiceType.Equals("Radiology"))
                        .Select(i => i.Code)
                        .ToList();


                        var serviceDelItemAdditionRadiologyCodes = servicesReplace
                         .Where(x => x.ReplaceCusHISServiceType != null && x.ReplaceCusHISServiceType.Equals("Investigation"))
                         .SelectMany(x => x.ItemAddition)
                         .Where(i => i != null && i.ClientServiceType.Equals("Radiology"))
                         .Select(i => i.Code)
                         .ToList();

                        if (serviceDelItemRemovalRadiologyCodes.Any())
                        {
                            serviceGridDtoList.RemoveByCodes(serviceDelItemRemovalRadiologyCodes);
                        }
                        if (serviceDelItemAdditionRadiologyCodes.Any())
                        {
                            //Assuming 'dxinfoesAdd' is a collection you're iterating through
                            List<ServicesGridDto> newData = serviceDelItemAdditionRadiologyCodes.Select(i => new ServicesGridDto { Code = i, Type = "Radiology" }).ToList();

                            //Add the new data to the existing list in the singleton instance
                            serviceGridDtoList.AddServiceList(newData);

                            AddAndDeleteCPTInRadiology(servicesGridDtos.Where(x => x.Type == "Radiology").Select(x => x.Code).ToList(), beforeDeletionRadiologyItemCount, deletionRequired: true);
                        }

                        //// labortory case 
                        var serviceDelItemRemovalLabortoryCodes = servicesReplace
                          .Where(x => x.ReplaceCusHISServiceType != null && x.ReplaceCusHISServiceType.Equals("Investigation"))
                         .SelectMany(x => x.ItemRemoval)
                         .Where(i => i != null && i.ClientServiceType.Equals("Laboratory"))
                         .Select(i => i.Code)
                         .ToList();

                        //// labortory case 
                        var serviceDelItemAdditionLabortoryCodes = servicesReplace
                          .Where(x => x.ReplaceCusHISServiceType != null && x.ReplaceCusHISServiceType.Equals("Investigation"))
                         .SelectMany(x => x.ItemAddition)
                         .Where(i => i != null && i.ClientServiceType.Equals("Laboratory"))
                         .Select(i => i.Code)
                         .ToList();

                        if (serviceDelItemRemovalLabortoryCodes.Any())
                        {
                            serviceGridDtoList.RemoveByCodes(serviceDelItemRemovalLabortoryCodes);
                        }
                        if (serviceDelItemAdditionLabortoryCodes.Any())
                        {
                            //Assuming 'dxinfoesAdd' is a collection you're iterating through
                            List<ServicesGridDto> newData = serviceDelItemAdditionLabortoryCodes.Select(i => new ServicesGridDto { Code = i, Type = "Laboratory" }).ToList();

                            //Add the new data to the existing list in the singleton instance
                            serviceGridDtoList.AddServiceList(newData);

                            AddAndDeleteCPTInLabOrder(servicesGridDtos.Where(x => x.Type == "Laboratory").Select(x => x.Code).ToList(), beforeDeletionLaboratoryItemCount, deletionRequired: true);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
                //this.Close();
            }
            RuleValidationOperationFinished.Invoke();
        }
        #endregion accept button click handler code goes here

    }
}
