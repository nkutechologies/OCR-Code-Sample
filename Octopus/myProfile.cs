using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Common;
using Octopus.Dtos.Common;
using Octopus.Dtos.ConditionsDtos;
using Octopus.Dtos.CPTSuggestionDtos;
using Octopus.Dtos.PatientConditionsDtos;
using Octopus.Dtos.PatientDtos;
using Octopus.Dtos.ValidateRulesDtos;
using Octopus.Helpers;
using Octopus.Helpers.CustomHelpers;
using Octopus.Helpers.CustomHelpers.Extensions;
using Octopus.Models.Singleton.DignosisGridDtos;
using Octopus.Models.Singleton.ServicesGridDtos;
using Octopus.Services.CPTSuggestionServices;
using Octopus.Services.PatientConditionsServices;
using Octopus.Services.ValidateRulesServices;

namespace Octopus
{
    public partial class myProfile : BaseForm
    {
        private bool isOctopusIconDraggable = false;

        private bool isFormDragging = false;
        private Point lastCursorPosition;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isFormDragging = true;
                lastCursorPosition = Control.MousePosition;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isFormDragging)
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
                isFormDragging = false;
            }
        }

        //private delegate void ExecutionDelegate(JObject config);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        const byte VK_CONTROL = 0x11;
        const byte VK_C = 0x43;
        const uint KEYEVENTF_KEYUP = 0x0002;

        static log4net.ILog _log = log4net.LogManager.GetLogger(typeof(myProfile));

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

        private const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_DOUBLECLICK = 0x0002; // double click flag

        public myProfile()
        {
            InitializeComponent();
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;


            this.ShowInTaskbar = false; // Ensure it doesn't show in the taskbar
            this.TopMost = true; // Ensure it's displayed above other windows
            this.Activate(); // Ensure the dialog gains focus
            this.BringToFront();
            SetForegroundWindow(this.Handle);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Makes the form non-resizable

            // Remove the maximize box (button) from the window style
            int currentStyle = GetWindowLong(Handle, GWL_STYLE);
            int newStyle = currentStyle & ~WS_MAXIMIZEBOX;
            SetWindowLong(Handle, GWL_STYLE, newStyle);

            InitializeFloatingIconForm(); // Initialize the icon form

            HideThisForm();


            // Set the form border style to None
            this.FormBorderStyle = FormBorderStyle.None;
            // Set padding to create a space around the content
            this.Padding = new Padding(-65); // Adjust the value to your preference

            pictureBox5.Click += MinimizeButton_Click;


            pictureBox2.Click += CloseButton_Click;
        }
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HideThisForm()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width + 100, Screen.PrimaryScreen.WorkingArea.Height + 100);
            this.notifyIcon1.Visible = true;
        }
        Form iconForm = new Form();
        bool isDragging = false;
        Point lastCursor;
        PictureBox floatingIcon = new PictureBox();
        bool isOctopusIconOndefaultLocation = true;
        private void InitializeFloatingIconForm()
        {
            iconForm = new Form();
            iconForm.FormBorderStyle = FormBorderStyle.None;
            iconForm.ShowInTaskbar = false;
            iconForm.StartPosition = FormStartPosition.Manual;
            iconForm.ShowIcon = false; // Hide icon in taskbar
            iconForm.TransparencyKey = Color.Magenta; // Set transparency color

            iconForm.TopMost = true; // Set the icon form to always be on top

            // Set the form's size and location for the icon
            iconForm.Size = new Size(45, 45); // Decreased size for the background

            Panel backgroundPanel = new Panel();
            backgroundPanel.Size = iconForm.ClientSize;
            backgroundPanel.BackColor = ColorTranslator.FromHtml("#eaeaea");//Color.DarkGray; // Set the background color //#ececec #d7d7d7 #f0f0f0 #ececec #b7b7b7 #cacaca #E4E4E4 #f2f2f2 #f0f0f0 #e3e3e3

            backgroundPanel.BorderStyle = BorderStyle.None;
            backgroundPanel.Region = _helperService.GetRoundedRegion(backgroundPanel.ClientRectangle, 25); // Set fully rounded border


            floatingIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            floatingIcon.Size = new Size(33, 33); // Smaller image size within the background panel

            // Load your icon image with better quality
            string iconPath = "download-compresskaru.com.png";
            string currentDirectory = CurrentWorkingDirectory;
            string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", iconPath);
            try
            {
                Image iconImage = _helperService.LoadHighQualityImage(iconImagePath, floatingIcon.Width, floatingIcon.Height);
                floatingIcon.Image = iconImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }

            floatingIcon.Click += (sender, e) => FloatingIcon_Click(iconForm);

            // Center the PictureBox within the Panel
            floatingIcon.Location = new Point((backgroundPanel.Width - floatingIcon.Width) / 2, (backgroundPanel.Height - floatingIcon.Height) / 2);

            backgroundPanel.Controls.Add(floatingIcon);
            iconForm.Controls.Add(backgroundPanel);
            iconForm.BackColor = Color.Magenta; // Set the background color to the transparency key color

            isOctopusIconOndefaultLocation = false;


            var moveOctopusIconToDignosisPlusSignSectionData = _configReaderService.GetSectionData("MoveOctopusIconToDignosisPlusSign", _config, true, false, false);

            if (moveOctopusIconToDignosisPlusSignSectionData != null)
            {
                int initialX = (int)moveOctopusIconToDignosisPlusSignSectionData.Coordinates.InitialX;
                int initialY = (int)moveOctopusIconToDignosisPlusSignSectionData.Coordinates.InitialY;

                iconForm.Location = new Point(initialX, initialY);
            }
            else
            {
                _log.Info("couldn't find section with name moveOctopusIconToDignosisPlusSignSectionData in config file");
                MessageBox.Show("Something went wrong, please contact system's admninistrator");
            }

            iconForm.Show();
        }
        private Form CreateChildForm(string imagePath, int posX, int posY)
        {
            Form childForm = new Form();
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.StartPosition = FormStartPosition.Manual;
            childForm.ShowInTaskbar = false;
            childForm.Size = new Size(45, 45);

            // Create a circular region for the form
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, childForm.Width, childForm.Height);
            childForm.Region = new Region(path);

            Panel childPanel = new Panel();
            childPanel.Size = childForm.ClientSize;
            childPanel.BackColor = Color.Transparent;
            childPanel.BorderStyle = BorderStyle.None;

            PictureBox pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Size = new Size(33, 33);

            try
            {
                string currentDirectory = CurrentWorkingDirectory;
                string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", imagePath);

                Image image = Image.FromFile(iconImagePath);
                pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }

            pictureBox.Click += (sender, e) => PictureBox_Click(imagePath);

            pictureBox.Location = new Point((childPanel.Width - pictureBox.Width) / 2, (childPanel.Height - pictureBox.Height) / 2);

            childPanel.Controls.Add(pictureBox);
            childForm.Controls.Add(childPanel);

            childForm.Location = new Point(posX, posY);

            return childForm;
        }

        private List<Form> childForms = new List<Form>();

        private void FloatingIcon_Click(Form parentForm)
        {
            List<string> imagePaths = new List<string> { "X.png", "RULES.png"/*, "CPT.png"*/, "ICD.png" };
            int formsOffset = 5;
            int childFormHeight = 45;

            CloseChildFloatingIcons();

            int spaceAbove = parentForm.Top;
            int spaceBelow = Screen.PrimaryScreen.WorkingArea.Height - parentForm.Bottom;

            int posX = parentForm.Left;
            int posY = parentForm.Top - childFormHeight - formsOffset; // Initial position of the first child form

            foreach (string imagePath in imagePaths)
            {
                Form childForm = CreateChildForm(imagePath, posX, posY);
                childForms.Add(childForm);
                posY -= childFormHeight + formsOffset; // Adjust the posY for each iteration
            }

            int totalChildFormsHeight = childForms.Count * (childFormHeight + formsOffset);

            if (totalChildFormsHeight < spaceBelow)
            {
                // Open the child forms downwards
                posY = parentForm.Bottom + formsOffset;

                foreach (var childForm in childForms)
                {
                    posY += formsOffset;
                    childForm.Location = new Point(posX, posY);
                    childForm.Show();
                    posY += childFormHeight;
                }
            }
            else if (totalChildFormsHeight < spaceAbove)
            {
                // Open the child forms upwards
                posY = parentForm.Top - totalChildFormsHeight - formsOffset;

                foreach (var childForm in childForms)
                {
                    posY += formsOffset;
                    childForm.Location = new Point(posX, posY);
                    childForm.Show();
                    posY += childFormHeight;
                }
            }
            else
            {
                MessageBox.Show("Not enough space to open child forms.");
            }
        }

        private void FloatingIcon_MouseDown(object sender, MouseEventArgs e)
        {
            CloseChildFloatingIcons();
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
            }
        }

        private void FloatingIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point delta = new Point(Cursor.Position.X - lastCursor.X, Cursor.Position.Y - lastCursor.Y);
                iconForm.Location = new Point(iconForm.Location.X + delta.X, iconForm.Location.Y + delta.Y);
                lastCursor = Cursor.Position;
            }
        }

        private void FloatingIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        #region Child icons for icd,cpt and ruleValidation goes here
        private void PictureBox_Click(string text)
        {
            //Child icons click handler code goes here

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
            if (fileNameWithoutExtension == "X")
            {
                // Event handler for Close icon click
                CloseChildFloatingIcons();
            }
            if (fileNameWithoutExtension == "RULES")
            {
                CloseChildFloatingIcons();

                // Event handler for Rules icon click
                RulesBtn_Clicked();
            }
            if (fileNameWithoutExtension == "CPT")
            {
                CloseChildFloatingIcons();

                // Event handler for CPT icon click
                ProcessMethodBasedOnConfig(_config, CptBtn_Clicked);
            }
            if (fileNameWithoutExtension == "ICD")
            {
                CloseChildFloatingIcons();

                // Event handler for ICD icon click
                ProcessMethodBasedOnConfig(_config, IcdBtn_Clicked);
            }
        }
        #endregion
        private string ReadXmlData(JObject config)
        {
            ClickOnOtherActions(config);
            ClickOnCeedValidation(config);
            ClickOnViewXML(config);
            ClickOnDownloadXML(config);
            SimulateCopyEvent(config);

            var xmlFileName = GetDataFromClipboard(config);

            CopyFolderPath(config);
            var directoryPath = GetDataFromClipboard(config);

            if (Path.GetExtension(xmlFileName).Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                _log.Info("The file has a .xml extension.");
            }
            else
            {
                _log.Info("The file does not have a .xml extension so concatinate it.");
                xmlFileName = xmlFileName + ".xml";
            }

            var fullFilePath = Path.Combine(directoryPath, xmlFileName);
            SaveXMLFile(config);
            CloseXMLViewer(config);
            CloseCEEDValidationWindow(config);

            return fullFilePath;
        }

        private void CptBtn_Clicked(JObject config)
        {
            #region reading data from xml code starts here
            var fullFilePath = ReadXmlData(config);
            #endregion reading data from xml code ends here

            var cptIcdAndDOb = GetCptAndIcdCodesAndDOBByXML(fullFilePath, config);

            if (cptIcdAndDOb != null && cptIcdAndDOb.Dignoses.Any())
            {
                HandleApiCall(() => CPTSuggestionService.GetCPTSuggestion(
                JsonConvert.SerializeObject(new CPTSuggestionspayLoadDto
                {
                    icd = cptIcdAndDOb.Dignoses.Select(x => x.ICDCode).ToList(),
                    dob = cptIcdAndDOb.PatientPersonalInfo.DOB
                }), AuthToken, _apiBaseURl), ShowCPTSuggestions);

            }
            else
            {
                //rather than showing empty cpt suggestion form show messagebox
                MessageBox.Show("Please enter dignosis before requesting cpt suggestions");
                _log.Info("Please enter dignosis before requesting cpt suggestions");
            }
        }
        private void IcdBtn_Clicked(JObject config)
        {
            if (!isOctopusIconOndefaultLocation)
            {
                iconForm.Hide();
            }

            MouseScrollUp();

            //fetch patient chief complaint,dob,gender
            var patientConditionsDto = FetchPatientPersonalInfo(config);

            if (patientConditionsDto != null && !string.IsNullOrEmpty(patientConditionsDto.Text))
            {
                HandleApiCall(() => PatientConditionsService.GetPatientConditions(JsonConvert.SerializeObject(patientConditionsDto), _apiBaseURl, AuthToken), ShowICDSuggestions);
            }
            else
            {
                MessageBox.Show("Something went wrong, couldn't capture data, Please try again");
                _log.Error("Something went wrong, couldn't capture data, Please try again");
            }
        }
        private PatientConditionsDto FetchPatientPersonalInfo(JObject config)
        {
            iconForm.Hide();

            var patientConditionsDto = new PatientConditionsDto();
            var patientMpi = FetchPatientMPI();
            var previousMpiObj = GetStoredPatientMpi();

            void UpdatePatientConditionsDto(Models.Singleton.PatientConditionsDtos.PatientConditionsDto sourceDto)
            {
                patientConditionsDto.Text = sourceDto.Text;
                patientConditionsDto.DOB = sourceDto.DOB;
                patientConditionsDto.Gender = sourceDto.Gender;
            }

            if (previousMpiObj != null && patientMpi.Equals(previousMpiObj.MPI))
            {
                isOctopusIconOndefaultLocation = true;
                var patientConditionsSingletonObject = GetPatientConditionsFromSingleton();

                if (patientConditionsSingletonObject != null)
                {
                    UpdatePatientConditionsDto(patientConditionsSingletonObject);
                    return patientConditionsDto;
                }
            }
            isOctopusIconOndefaultLocation = false;

            AddToPatientMpiSingleton(new Models.Singleton.PatientMpiDtos.PatientMpiDto { MPI = patientMpi });
            patientConditionsDto = FetchICDSuggestionsData(config);
            AddToPatientConditionsSingleton(new Models.Singleton.PatientConditionsDtos.PatientConditionsDto
            {
                DOB = patientConditionsDto.DOB,
                Gender = patientConditionsDto.Gender,
                Text = patientConditionsDto.Text
            });

            return patientConditionsDto;
        }
        private ICDSuggestions icdSuggestionsForm; // Declare icdSuggestionsForm at the class level
        private void ShowICDSuggestions(ResponseModel<List<GroupedConditionsDto>> response)
        {
            // Deserialize the JSON string back into a .NET object
            PatientConditionsDto deserializedObject = JsonConvert.DeserializeObject<PatientConditionsDto>(response.Request);
            if (response.Data.Any())
            {
                icdSuggestionsForm = new ICDSuggestions(response.Data, deserializedObject);
                icdSuggestionsForm.IcdOperationFinished += OnIcdOperationFinished;
                ////show form
                //// Ensure the form is shown in the foreground and active
                icdSuggestionsForm.ShowDialog();
                icdSuggestionsForm.Activate();
            }
            else
            {
                MessageBox.Show("Can't find any suggestions, Please try again");
            }
        }
        private void OnIcdOperationFinished()
        {
            if (!iconForm.Visible && !isOctopusIconOndefaultLocation)
            {
                var moveOctopusIconToMarkToBillBtnSectionData = _configReaderService.GetSectionData("MoveOctopusIconToMarkToBillBtn", _config, true, false, false);

                if (moveOctopusIconToMarkToBillBtnSectionData != null)
                {
                    int initialX = (int)moveOctopusIconToMarkToBillBtnSectionData.Coordinates.InitialX;
                    int initialY = (int)moveOctopusIconToMarkToBillBtnSectionData.Coordinates.InitialY;

                    iconForm.Location = new Point(initialX, initialY);

                    if (isOctopusIconDraggable)
                    {
                        isOctopusIconDraggable = false;
                        // Attach PictureBox-level mouse events to enable dragging
                        floatingIcon.MouseDown -= FloatingIcon_MouseDown;
                        floatingIcon.MouseMove -= FloatingIcon_MouseMove;
                        floatingIcon.MouseUp -= FloatingIcon_MouseUp;
                    }
                }

            }
            iconForm.Show();
            icdSuggestionsForm.IcdOperationFinished -= OnIcdOperationFinished;
        }
        private void ShowCPTSuggestions(ResponseModel<List<CPTSuggestionsResponseDto>> response)
        {
            CPTSuggestions cptSuggestionsForm = new CPTSuggestions(response.Data);
            // Show form
            cptSuggestionsForm.ShowDialog();
            cptSuggestionsForm.Activate();
        }

        private PatientConditionsDto FetchICDSuggestionsData(JObject config)
        {
            var patientConditionsDto = new PatientConditionsDto();

            if (_appSettings.Configurations.IsNotificationEnabled)
            {
                _helperService.ShowNotification("Octopus started!");
            }

            ClickOnSoapNotes(config);

            var gender = string.Empty;
            DateTime dob = new DateTime();

            var capturedText = FetchGender(config);
            CloseSoapNotes(config);

            if (!string.IsNullOrEmpty(capturedText))
            {
                string[] capturedArray = capturedText.Split(',');
                if (capturedArray.Length > 1)
                {
                    var arr = capturedText.Split(',');
                    gender = arr[1].Replace(" ", "");
                    var age = arr[2];
                    dob = age.CalculateDOB();
                }
            }

            ClickOnObservation(config);
            ClickOnComplain(config);
            var chiefComplaint = FetchChiefComplaint(config);

            if (!string.IsNullOrEmpty(chiefComplaint))
            {
                chiefComplaint = chiefComplaint.Replace(",", ".");
            }

            CloseObservation(config);

            patientConditionsDto.Gender = gender;
            patientConditionsDto.Text = chiefComplaint;
            patientConditionsDto.DOB = dob.ToString("MM/dd/yyyy");
            return patientConditionsDto;
        }
        private void CloseChildFloatingIcons()
        {
            // Close child forms on dragging
            foreach (var form in childForms)
            {
                form.Close();
            }
            childForms.Clear(); // Clear the list of child forms
        }
        private void myProfile_Load(object sender, EventArgs e)
        {
            //// Set the form size to a specific width and height
            //label3.Font = new Font("Arial", 8, FontStyle.Bold);
            //// Set the label's ForeColor property to the created color
            //label3.ForeColor = ColorTranslator.FromHtml("#302168");
            //label3.BackColor = Color.Transparent;
            //label3.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA";

            materialLabel1.Font = new Font("Montserrat", 9, FontStyle.Regular);
            materialLabel1.BackColor = Color.Transparent;
            materialLabel1.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA";

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();


            label2.Font = new Font(label2.Font.FontFamily, label2.Font.Size + 1/*, FontStyle.Regular*/);

            _helperService.CustomizePanel(panel1);

            _helperService.CustomizeLabel(label1);

            var sizeOfScreen = CurrentDisplayResolution;

            // Check the screen resolution and apply adjustments accordingly
            if (sizeOfScreen.Width <= 1400 && sizeOfScreen.Height <= 1050)
            {
                pictureBox1.Width = this.ClientSize.Width + 10;
                pictureBox1.Height = this.ClientSize.Height + 4;
                pictureBox5.Location = new Point(pictureBox5.Location.X - 32, pictureBox5.Location.Y);
                pictureBox2.Location = new Point(pictureBox2.Location.X - 32, pictureBox2.Location.Y);
            }
        }

        private void myProfile_Resize(object sender, EventArgs e)
        {
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

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Show();
                this.WindowState = FormWindowState.Normal;
            }
            if (this.Location == new Point(Screen.PrimaryScreen.WorkingArea.Width + 100, Screen.PrimaryScreen.WorkingArea.Height + 100))
            {
                // Checking if the form's location is at a specific position
                this.StartPosition = FormStartPosition.Manual;

                // Centering the form in the screen
                int centerX = Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;
                int centerY = Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
                this.Location = new Point(centerX, centerY);
            }
        }

        #region Refresh button click handler code goes here
        private void pictureBox4_Click(object sender, EventArgs e)
        {

            // Clear the stored PatientConditionsDto
            Models.Singleton.PatientConditionsDtos.PatientConditionsDtoObject.GetInstance().ClearPatientConditions();

            // Clear the stored PatientMpiDto
            Models.Singleton.PatientMpiDtos.Octopus.Models.Singleton.PatientMpiDtos.PatientMpiDtoObject.GetInstance().ClearPatientMpi();

            //// Hide or close the iconForms here
            CloseChildFloatingIcons();
            iconForm.Close();

            if (_appSettings.Configurations.IsNotificationEnabled)
            {
                _helperService.ShowNotification("Refreshed successfully");
            }
            this.Hide();
            Dispose(true);
            myProfile myProfile = new myProfile();
            myProfile.Show();
            MessageBox.Show("Refreshed successfully");

        }
        #endregion Refresh button click handler code goes here

        #region Logout button click handler code goes here
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            // Clear the stored PatientConditionsDto
            Models.Singleton.PatientConditionsDtos.PatientConditionsDtoObject.GetInstance().ClearPatientConditions();

            // Clear the stored PatientMpiDto
            Models.Singleton.PatientMpiDtos.Octopus.Models.Singleton.PatientMpiDtos.PatientMpiDtoObject.GetInstance().ClearPatientMpi();

            //reset auth token with empty string
            AppSettingManager.Reset("authToken");

            this.notifyIcon1.Visible = false;

            //// Hide or close the iconForms here
            CloseChildFloatingIcons();
            iconForm.Close();

            this.Hide();
            Dispose(true);
            Login myLogin = new Login();
            myLogin.ShowDialog();
        }
        #endregion Logout button click handler code goes here

        private void ExecuteOctopusForRuleValidator(JObject config)
        {
            try
            {
                if (!isOctopusIconOndefaultLocation)
                {
                    iconForm.Hide();
                }

                MouseScrollUp();

                if (_appSettings.Configurations.IsNotificationEnabled)
                {
                    _helperService.ShowNotification("Octopus started!");
                }

                #region capturing chief complaint,gender,dob
                //started capturing chief complaint,gender,dob
                PatientConditionsDto patientConditionsDto = FetchPatientPersonalInfo(config);
                //finished capturing chief complaint,gender,dob
                #endregion capturing chief complaint,gender,dob

                #region reading data from xml code starts here
                //reading data from xml starts here
                var fullFilePath = ReadXmlData(config);
                #endregion reading data from xml code ends here

                var cptIcdAndDOb = GetCptAndIcdCodesAndDOBByXML(fullFilePath, config);
                if (cptIcdAndDOb == null)
                {
                    MessageBox.Show("Something went wrong, please try again later");
                    OnRuleValidationOperationFinished();
                }
                // Using the ParseExactInvariant method for parsing the dates
                DateTime startDate = cptIcdAndDOb.Encounter.StartDate.ParseExactInvariant("dd/MM/yyyy HH:mm");
                DateTime endDate = cptIcdAndDOb.Encounter.EndDate.ParseExactInvariant("dd/MM/yyyy HH:mm");

                // Using the ToFormattedString method to convert back to string with the desired format
                cptIcdAndDOb.Encounter.StartDate = startDate.ToFormattedString("dd/MM/yyyy HH:mm", "dd/MM/yyyy");
                cptIcdAndDOb.Encounter.EndDate = endDate.ToFormattedString("dd/MM/yyyy HH:mm", "dd/MM/yyyy");


                ValidateRules(cptIcdAndDOb, patientConditionsDto.Gender, patientConditionsDto.Text);
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private void RulesBtn_Clicked()
        {
            try
            {
                ProcessMethodBasedOnConfig(_config, ExecuteOctopusForRuleValidator);
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private void ProcessMethodBasedOnConfig(JObject config, Action<JObject> executeMethod)
        {
            if (_appSettings.Configurations.IsDoctorsDeskScreenEnabled && _appSettings.Configurations.IsInsuranceTypeCustomerEnabled)
            {
                if (_appSettings.Configurations.IsNotificationEnabled)
                {
                    _helperService.ShowNotification("Please Wait...");
                }

                if (!isCurrentScreenLTDoctorsDesk())
                {
                    if (_appSettings.Configurations.IsNotificationEnabled)
                    {
                        _helperService.ShowNotification("Access Restricted");
                    }

                    // Alternatively, to make it a top-level window across all screens:
                    MessageBox.Show(this, "Please select Doctor's Desk in LifeTrenz.", "Octopus", MessageBoxButtons.OK);
                }
                else
                {
                    CheckCustomerTypeAndRunOctopus(config, executeMethod);

                }
            }
            else if (_appSettings.Configurations.IsDoctorsDeskScreenEnabled && !_appSettings.Configurations.IsInsuranceTypeCustomerEnabled)
            {
                if (_appSettings.Configurations.IsNotificationEnabled)
                {
                    _helperService.ShowNotification("Please Wait...");
                }
                if (!isCurrentScreenLTDoctorsDesk())
                {
                    if (_appSettings.Configurations.IsNotificationEnabled)
                    {
                        _helperService.ShowNotification("Access Restricted");
                    }
                    // Alternatively, to make it a top-level window across all screens:
                    MessageBox.Show(this, "Please select Doctor's Desk in LifeTrenz.", "Octopus", MessageBoxButtons.OK);
                }
                else
                {
                    executeMethod(config);
                }
            }
            else if (_appSettings.Configurations.IsInsuranceTypeCustomerEnabled && !_appSettings.Configurations.IsDoctorsDeskScreenEnabled)
            {
                CheckCustomerTypeAndRunOctopus(config, executeMethod);
            }
            else if (!_appSettings.Configurations.IsDoctorsDeskScreenEnabled && !_appSettings.Configurations.IsInsuranceTypeCustomerEnabled)
            {
                executeMethod(config);
            }

        }
        private void CheckCustomerTypeAndRunOctopus(JObject config, Action<JObject> executeMethod /*ExecutionDelegate executionMethod*/)
        {
            // Implement the action upon clicking the floating icon
            try
            {

                ClickOnOtherActions(config);

                ClickOnFollowUp(config);

                // Clear the clipboard
                ClipboardManager.Clear();

                CopyPayerTypeForSelfPayer(config);

                var payerType = ClipboardManager.GetClipBoardContent();


                if (string.IsNullOrEmpty(payerType))
                {
                    payerType = BookAppointment(config);

                    if (IsInsuranceTypeCustomer(payerType))
                    {
                        //run octopus logic for validating rules
                        //ExecuteOctopus(config);
                        //executionMethod?.Invoke(config);
                        executeMethod(config);
                    }
                    else
                    {
                        //MessageBox.Show(this, "You cannot validate self pay type patients data", "Octopus", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    //implement checks for payer type if insurance run octopus if not show alert err msg
                    //payerType
                    if (IsInsuranceTypeCustomer(payerType))
                    {
                        //close Follow up visits window
                        CloseFollowUpForSelfPayer(config);
                        //run octopus logic for validating rules
                        //ExecuteOctopus(config);
                        //executionMethod?.Invoke(config);
                        executeMethod(config);
                    }
                    else
                    {
                        //close Follow up visits window
                        CloseFollowUpForSelfPayer(config);
                        //MessageBox.Show("You cannot validate self pay type patients data");
                    }
                }
                if (string.IsNullOrEmpty(payerType))
                {
                    //show alert that your appointment status is finalized, can't run validator
                    // Alternatively, to make it a top-level window across all screens:
                    MessageBox.Show(this, "Either Your appointment status is finalized or you need to add OT Procedure Order", "Octopus", MessageBoxButtons.OK);
                    if (_appSettings.Configurations.IsNotificationEnabled)
                    {
                        _helperService.ShowNotification("Either Your appointment status is finalized or you need to add OT Procedure Order");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private void ClickOnCeedValidation(JObject config)
        {
            try
            {
                var orderBasketSectionData = _configReaderService.GetSectionData("ClickOnCeedValidation", config, true, false, false);

                if (orderBasketSectionData != null)
                {
                    int initialX = (int)orderBasketSectionData.Coordinates.InitialX;
                    int initialY = (int)orderBasketSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)orderBasketSectionData.InitialDelay;
                    //int dragDistance = (int)orderBasketSectionData.Drag.Distance;
                    //int loopIncrementBy = (int)orderBasketSectionData.Drag.loopIncrementBy;
                    //int speedOfDrag = (int)orderBasketSectionData.Drag.SpeedOfDrag;

                    Thread.Sleep(initialThreadSleepDuration);

                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

                    //var finalPosition = Cursor.Position;
                    //for (int i = 0; i < dragDistance; i += loopIncrementBy)
                    //{
                    //    Cursor.Position = new Point(initialX, initialY + i);
                    //    Thread.Sleep(speedOfDrag);
                    //    finalPosition = Cursor.Position;
                    //}

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
                else
                {
                    _log.Info("ClickOnOrderBasket configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void ClickOnSoapNotes(JObject config)
        {
            try
            {
                var soapNotesSectionData = _configReaderService.GetSectionData("ClickOnSoapNotes", config, true, false, false);

                if (soapNotesSectionData != null)
                {
                    int initialThreadSleepDuration = (int)soapNotesSectionData.InitialDelay;
                    int initialX = (int)soapNotesSectionData.Coordinates.InitialX;
                    int initialY = (int)soapNotesSectionData.Coordinates.InitialY;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
                else
                {
                    _log.Info("ClickOnSoapNotes configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private string FetchGender(JObject config)
        {
            try
            {
                // Clear the clipboard
                //Clipboard.Clear();

                var fetchGenderSectionData = _configReaderService.GetSectionData("FetchGender", config, true, false, false);

                if (fetchGenderSectionData != null && fetchGenderSectionData.SectionConfig != null)
                {
                    int initialX = (int)fetchGenderSectionData.Coordinates.InitialX;
                    int initialY = (int)fetchGenderSectionData.Coordinates.InitialY;
                    int dragDistance = (int)fetchGenderSectionData.SectionConfig["Drag"]["Distance"];
                    //int maxYLimit = (int)fetchGenderSectionData.SectionConfig["Drag"]["MaxYLimit"]; // Adjusted for right-to-left drag
                    int speedOfDrag = (int)fetchGenderSectionData.SectionConfig["Drag"]["SpeedOfDrag"];
                    int loopIncrementBy = (int)fetchGenderSectionData.SectionConfig["Drag"]["loopIncrementBy"];
                    int initialThreadSleepDuration = (int)fetchGenderSectionData.InitialDelay;
                    int afterDragSleepDuration = (int)fetchGenderSectionData.SectionConfig["AfterDragSleep"]["Halt"];
                    int afterCopySleepDuration = (int)fetchGenderSectionData.SectionConfig["AfterCopySleep"]["Halt"];

                    Thread.Sleep(initialThreadSleepDuration);

                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    //Thread.Sleep(afterDragSleepDuration);

                    int endX = dragDistance; // Initial X position for right-to-left movement
                    int currentX = initialX;

                    //perform drag action
                    while (currentX > endX)
                    {
                        currentX -= loopIncrementBy; // Move 10 pixels to the left
                        SetCursorPos(currentX, initialY);
                        Thread.Sleep(speedOfDrag); // Adjust the speed here (milliseconds)
                    }

                    mouse_event(MOUSEEVENTF_LEFTUP, currentX, initialY, 0, 0); // Left mouse button up

                    // Uncomment to perform drag action
                    // for (int i = 0; i < dragDistance; i += loopIncrementBy)
                    // {
                    //     if (initialY + i <= maxYLimit)
                    //     {
                    //         Cursor.Position = new Point(initialX, initialY + i);
                    //         Thread.Sleep(speedOfDrag);
                    //     }
                    //     else
                    //     {
                    //         break;
                    //     }
                    // }

                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY + dragDistance, 0, 0);
                    Thread.Sleep(afterDragSleepDuration);

                    SendKeys.SendWait("^{c}"); // Simulate Ctrl+C (copy)
                    Thread.Sleep(afterCopySleepDuration);
                    if (Clipboard.ContainsText())
                    {
                        return Clipboard.GetText().Replace("aged ", "").Replace("\r\n\r\n", "");
                    }
                    else
                    {
                        // Handle if no text is found in the clipboard
                        return string.Empty;
                    }
                }
                else
                {
                    _log.Info("FetchGender configuration not found in the JSON.");
                    return string.Empty;
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
                return string.Empty;
            }
        }

        private void CloseSoapNotes(JObject config)
        {
            try
            {
                var closeSoapNotesSectionData = _configReaderService.GetSectionData("CloseSoapNotes", config, true, false, false);

                if (closeSoapNotesSectionData.SectionConfig != null && closeSoapNotesSectionData != null)
                {
                    int initialX = (int)closeSoapNotesSectionData.Coordinates.InitialX;
                    int initialY = (int)closeSoapNotesSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)closeSoapNotesSectionData.InitialDelay;
                    int beforeDoubleClickThreadSleepDuration = (int)closeSoapNotesSectionData.SectionConfig["BeforeDoubleClickThreadSleeper"]["Halt"];

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

                    Thread.Sleep(beforeDoubleClickThreadSleepDuration);

                    mouse_event(MOUSEEVENTF_DOUBLECLICK, initialX, initialY, 0, 0);
                }
                else
                {
                    _log.Info("CloseSoapNotes configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void ClickOnObservation(JObject config)
        {
            try
            {
                var clickOnObservationSectionData = _configReaderService.GetSectionData("ClickOnObservation", config, true, true, false);

                if (clickOnObservationSectionData != null)
                {
                    int initialX = (int)clickOnObservationSectionData.Coordinates.InitialX;
                    int initialY = (int)clickOnObservationSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)clickOnObservationSectionData.InitialDelay;
                    int betweenClicksThreadSleepDuration = (int)clickOnObservationSectionData.DoubleClickDelay;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

                    Thread.Sleep(betweenClicksThreadSleepDuration);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
                else
                {
                    _log.Info("ClickOnObservation configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void ClickOnComplain(JObject config)
        {
            try
            {
                var clickOnComplainSectionData = _configReaderService.GetSectionData("ClickOnComplain", config, true, true, false);

                if (clickOnComplainSectionData != null)
                {
                    int initialX = (int)clickOnComplainSectionData.Coordinates.InitialX;
                    int initialY = (int)clickOnComplainSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)clickOnComplainSectionData.InitialDelay;
                    int betweenClicksThreadSleepDuration = (int)clickOnComplainSectionData.DoubleClickDelay;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

                    Thread.Sleep(betweenClicksThreadSleepDuration);

                    //mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    //mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
                else
                {
                    _log.Info("ClickOnComplain configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }

        private string FetchChiefComplaint(JObject config)
        {
            // Clear the clipboard
            //Clipboard.Clear();
            try
            {
                var fetchChiefComplaintSectionData = _configReaderService.GetSectionData("FetchChiefComplaint", config, true, true, true);
                if (fetchChiefComplaintSectionData.SectionConfig != null && fetchChiefComplaintSectionData != null)
                {
                    int initialX = (int)fetchChiefComplaintSectionData.Coordinates.InitialX;
                    int initialY = (int)fetchChiefComplaintSectionData.Coordinates.InitialY;
                    int copyOptionX = (int)fetchChiefComplaintSectionData.contextMenuActions.InitialX;
                    int copyOptionY = (int)fetchChiefComplaintSectionData.contextMenuActions.InitialY;

                    int initialThreadSleepDuration = (int)fetchChiefComplaintSectionData.InitialDelay;
                    int firstLeftClickDelay = (int)fetchChiefComplaintSectionData.SectionConfig["FirstLeftClickDelay"]["Halt"];
                    int secondLeftClickDelay = (int)fetchChiefComplaintSectionData.SectionConfig["SecondLeftClickDelay"]["Halt"];
                    int doubleClickDelay = (int)fetchChiefComplaintSectionData.DoubleClickDelay;
                    int rightClickDelay = (int)fetchChiefComplaintSectionData.SectionConfig["RightClickDelay"]["Halt"];
                    int contextMenuDelay = (int)fetchChiefComplaintSectionData.SectionConfig["ContextMenuDelay"]["Halt"];
                    int afterCopyDelay = (int)fetchChiefComplaintSectionData.SectionConfig["AfterCopyDelay"]["Halt"];

                    Thread.Sleep(initialThreadSleepDuration);

                    string chiefComplaint = string.Empty;

                    int targetX = initialX;
                    int targetY = initialY;

                    Cursor.Position = new Point(targetX, targetY);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, targetX, targetY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, targetX, targetY, 0, 0);
                    Thread.Sleep(doubleClickDelay);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, targetX, targetY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, targetX, targetY, 0, 0);
                    SendKeys.SendWait("^{a}");

                    mouse_event(MOUSEEVENTF_RIGHTDOWN, targetX, targetY, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, targetX, targetY, 0, 0);
                    Thread.Sleep(contextMenuDelay);

                    Cursor.Position = new Point(copyOptionX, copyOptionY);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, copyOptionX, copyOptionY, 0, 0);
                    Thread.Sleep(afterCopyDelay);

                    if (Clipboard.ContainsText())
                    {
                        string copiedRowText = Clipboard.GetText();
                        chiefComplaint = copiedRowText.Replace("Complaints :", "");
                    }

                    Console.WriteLine("Grid values:" + chiefComplaint);
                    return chiefComplaint;
                }
                else
                {
                    // Handle if configuration is not found
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log exceptions if any
                _log.Error(ex);
                return string.Empty;
            }
        }

        private void CloseObservation(JObject config)
        {
            try
            {
                var closeObservationSectionData = _configReaderService.GetSectionData("CloseObservation", config, true, true, false);
                if (closeObservationSectionData != null)
                {
                    int topRightX = (int)closeObservationSectionData.Coordinates.InitialX;
                    int topRightY = (int)closeObservationSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)closeObservationSectionData.InitialDelay;
                    int doubleClickDelay = (int)closeObservationSectionData.DoubleClickDelay;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(topRightX, topRightY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);

                    Thread.Sleep(doubleClickDelay); // Adjust the delay between clicks as needed

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);
                }
                else
                {
                    _log.Info("CloseObservation configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }

        private void ValidateRules(CptIcdAndDOb data, string gender, string chiefComplaint)
        {
            try
            {
                ValidateRulesDtos validateRulesDto = new ValidateRulesDtos();
                validateRulesDto.DxInfos = new List<DxInfo>();
                validateRulesDto.Services = new List<Service>();
                validateRulesDto.ReportContent = new List<ReportContent>();

                //add dignosis data into this shared singleton list for later use
                AddToDignosisGridSingelton(data.Dignoses.Select(x => new DiagnosisGridDto
                {
                    Code = x.ICDCode,
                    Type = x.ICDType
                }).ToList());

                //add services data into this shared singleton list for later use
                AddDataToServiceGridSingelton(data.CPT.Select(x => new ServicesGridDto
                {
                    Code = x.CPTCode,
                    Type = x.Type
                }).ToList());

                //get client's application's textboxes text before makin' api call 
                validateRulesDto.DOB = data.PatientPersonalInfo.DOB;
                validateRulesDto.Gender = gender;
                validateRulesDto.RequestId = data.PatientPersonalInfo.Id;
                validateRulesDto.Start = data.Encounter.StartDate;
                validateRulesDto.End = data.Encounter.EndDate;
                validateRulesDto.ChiefComplaint = chiefComplaint;

                validateRulesDto.DxInfos = data.Dignoses.Select(x => new DxInfo
                {
                    Code = x.ICDCode,
                    Type = x.ICDType
                }).ToList();

                validateRulesDto.Services = data.CPT.Select(x => new Service
                {
                    Code = x.CPTCode,
                    Qty = x.Quantity
                }).ToList();

                HandleApiCall(() => ValidateRulesService.Validate(JsonConvert.SerializeObject(validateRulesDto), AuthToken, _apiBaseURl), ShowValidationResults);

            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(ex);
            }
        }

        private RuleValidations ruleValidationForm; // Declare ruleValidationForm at the class level
        private Success successForm;
        private void ShowValidationResults(ResponseModel<List<RulesValidationResult>> result)
        {
            if (result.Data.Any())
            {
                ruleValidationForm = new RuleValidations(result.Data);
                ruleValidationForm.RuleValidationOperationFinished += OnRuleValidationOperationFinished;
                ruleValidationForm.ShowDialog();
            }
            else
            {
                successForm = new Success();
                successForm.SuccessFormClosed += OnSuccessFormClosed;
                successForm.ShowDialog();
            }
        }
        private void OnRuleValidationOperationFinished()
        {
            if (!iconForm.Visible)
            {
                isOctopusIconOndefaultLocation = true;

                iconForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - iconForm.Width, Screen.PrimaryScreen.WorkingArea.Height - iconForm.Height);
                if (!isOctopusIconDraggable)
                {
                    isOctopusIconDraggable = true;
                    // Attach PictureBox-level mouse events to enable dragging
                    floatingIcon.MouseDown += FloatingIcon_MouseDown;
                    floatingIcon.MouseMove += FloatingIcon_MouseMove;
                    floatingIcon.MouseUp += FloatingIcon_MouseUp;
                }
                iconForm.Show();
            }
            ruleValidationForm.RuleValidationOperationFinished -= OnRuleValidationOperationFinished;
        }
        private void OnSuccessFormClosed()
        {
            if (!iconForm.Visible)
            {
                isOctopusIconOndefaultLocation = true;

                iconForm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - iconForm.Width, Screen.PrimaryScreen.WorkingArea.Height - iconForm.Height);

                // Attach PictureBox-level mouse events to enable dragging
                floatingIcon.MouseDown += FloatingIcon_MouseDown;
                floatingIcon.MouseMove += FloatingIcon_MouseMove;
                floatingIcon.MouseUp += FloatingIcon_MouseUp;

                iconForm.Show();
            }
            successForm.SuccessFormClosed -= OnSuccessFormClosed;
        }
        //private void CloseOrderBasket(JObject config)
        //{
        //    try
        //    {
        //        var closeOrderBasketSectionData = _configReaderService.GetSectionData("CloseOrderBasket", config, true, false, true);
        //        if (closeOrderBasketSectionData != null)
        //        {
        //            int initialThreadSleepDuration = (int)closeOrderBasketSectionData.InitialDelay;
        //            int doubleClickDelay = (int)closeOrderBasketSectionData.DoubleClickDelay;
        //            int middleX = (int)closeOrderBasketSectionData.Coordinates.InitialX;
        //            int middleY = (int)closeOrderBasketSectionData.Coordinates.InitialY;

        //            Thread.Sleep(initialThreadSleepDuration);
        //            Cursor.Position = new Point(middleX, middleY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, middleX, middleY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, middleX, middleY, 0, 0);

        //            Thread.Sleep(doubleClickDelay); // Adjust the delay between clicks as needed

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, middleX, middleY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, middleX, middleY, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("CloseOrderBasket configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}
        //private void ClosePrintOrders(JObject config)
        //{
        //    try
        //    {
        //        var closePrintOrdersSectionData = _configReaderService.GetSectionData("ClosePrintOrders", config, true, false, true);
        //        if (closePrintOrdersSectionData != null)
        //        {
        //            int initialThreadSleepDuration = (int)closePrintOrdersSectionData.InitialDelay;
        //            int doubleClickDelay = (int)closePrintOrdersSectionData.DoubleClickDelay;
        //            int middleX = (int)closePrintOrdersSectionData.Coordinates.InitialX;
        //            int middleY = (int)closePrintOrdersSectionData.Coordinates.InitialY;

        //            Thread.Sleep(initialThreadSleepDuration);
        //            Cursor.Position = new Point(middleX, middleY);

        //            Console.WriteLine($"Cursor moved to: ({middleX}, {middleY})");

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, middleX, middleY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, middleX, middleY, 0, 0);

        //            Thread.Sleep(doubleClickDelay); // Adjust the delay between clicks as needed

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, middleX, middleY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, middleX, middleY, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("ClosePrintOrders configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}
        private void CloseCEEDValidationWindow(JObject config)
        {
            try
            {
                var closeCEEDValidationWindowSectionData = _configReaderService.GetSectionData("CloseCEEDValidationWindow", config, true, true, false);
                if (closeCEEDValidationWindowSectionData != null)
                {
                    int initialThreadSleepDuration = (int)closeCEEDValidationWindowSectionData.InitialDelay;
                    int doubleClickDelay = (int)closeCEEDValidationWindowSectionData.DoubleClickDelay;
                    int topRightX = (int)closeCEEDValidationWindowSectionData.Coordinates.InitialX;
                    int topRightY = (int)closeCEEDValidationWindowSectionData.Coordinates.InitialY;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(topRightX, topRightY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);

                    Thread.Sleep(doubleClickDelay); // Adjust the delay between clicks as needed

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);
                }
                else
                {
                    _log.Info("CloseCEEDValidationWindow configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void CloseXMLViewer(JObject config)
        {
            try
            {
                var closeXMLViewerSectionData = _configReaderService.GetSectionData("CloseXMLViewer", config, true, true, false);

                if (closeXMLViewerSectionData != null)
                {
                    int initialThreadSleepDuration = (int)closeXMLViewerSectionData.InitialDelay;
                    int doubleClickDelay = (int)closeXMLViewerSectionData.DoubleClickDelay;
                    int topRightX = (int)closeXMLViewerSectionData.Coordinates.InitialX;
                    int topRightY = (int)closeXMLViewerSectionData.Coordinates.InitialY;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(topRightX, topRightY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, (int)(uint)topRightX, (int)(uint)topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, (int)(uint)topRightX, (int)(uint)topRightY, 0, 0);

                    Thread.Sleep(doubleClickDelay); // Adjust the delay between clicks as needed

                    mouse_event(MOUSEEVENTF_LEFTDOWN, (int)(uint)topRightX, (int)(uint)topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, (int)(uint)topRightX, (int)(uint)topRightY, 0, 0);
                }
                else
                {
                    _log.Info("CloseXMLViewer configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        //private void ClickOnOrderBasket(JObject config)
        //{
        //    try
        //    {
        //        var orderBasketSectionData = _configReaderService.GetSectionData("ClickOnOrderBasket", config, true, true, false);

        //        if (orderBasketSectionData != null)
        //        {
        //            int initialX = (int)orderBasketSectionData.Coordinates.InitialX;
        //            int initialY = (int)orderBasketSectionData.Coordinates.InitialY;
        //            int initialThreadSleepDuration = (int)orderBasketSectionData.InitialDelay;
        //            int dragDistance = (int)orderBasketSectionData.Drag.Distance;
        //            int loopIncrementBy = (int)orderBasketSectionData.Drag.loopIncrementBy;
        //            int speedOfDrag = (int)orderBasketSectionData.Drag.SpeedOfDrag;

        //            Thread.Sleep(initialThreadSleepDuration);

        //            Cursor.Position = new Point(initialX, initialY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

        //            var finalPosition = Cursor.Position;
        //            for (int i = 0; i < dragDistance; i += loopIncrementBy)
        //            {
        //                Cursor.Position = new Point(initialX, initialY + i);
        //                Thread.Sleep(speedOfDrag);
        //                finalPosition = Cursor.Position;
        //            }

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, finalPosition.X, finalPosition.Y, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, finalPosition.X, finalPosition.Y, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("ClickOnOrderBasket configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}
        //private void ClickOnPrintOrders(JObject config)
        //{
        //    try
        //    {
        //        var printOrdersSectionData = _configReaderService.GetSectionData("ClickOnPrintOrders", config, true, false, true);

        //        if (printOrdersSectionData != null)
        //        {
        //            int initialX = (int)printOrdersSectionData.Coordinates.InitialX;
        //            int initialY = (int)printOrdersSectionData.Coordinates.InitialY;
        //            int initialThreadSleepDuration = (int)printOrdersSectionData.InitialDelay;
        //            int doubleClickDelay = (int)printOrdersSectionData.DoubleClickDelay;

        //            Thread.Sleep(initialThreadSleepDuration);

        //            Cursor.Position = new Point(initialX, initialY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

        //            Thread.Sleep(doubleClickDelay);
        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("ClickOnPrintOrders configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}
        //private void ClickOnCeedVerificationButton(JObject config)
        //{
        //    try
        //    {
        //        var ceedVerificationButtonSectionData = _configReaderService.GetSectionData("ClickOnCeedVerificationButton", config, true, false, true);
        //        if (ceedVerificationButtonSectionData != null)
        //        {
        //            int initialThreadSleepDuration = (int)ceedVerificationButtonSectionData.InitialDelay;
        //            int middleX = (int)ceedVerificationButtonSectionData.Coordinates.InitialX;
        //            int middleY = (int)ceedVerificationButtonSectionData.Coordinates.InitialY;
        //            int doubleClickDelay = (int)ceedVerificationButtonSectionData.DoubleClickDelay;

        //            Thread.Sleep(initialThreadSleepDuration);

        //            Cursor.Position = new Point(middleX, middleY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, middleX, middleY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, middleX, middleY, 0, 0);

        //            Thread.Sleep(doubleClickDelay);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, middleX, middleY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, middleX, middleY, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("ClickOnCeedVerificationButton configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}
        private void ClickOnViewXML(JObject config)
        {
            try
            {
                var clickOnViewXMLSectionData = _configReaderService.GetSectionData("ClickOnViewXML", config, true, true, false);
                if (clickOnViewXMLSectionData.SectionConfig != null && clickOnViewXMLSectionData != null)
                {
                    int initialThreadSleepDuration = (int)clickOnViewXMLSectionData.InitialDelay;
                    int topRightX = (int)clickOnViewXMLSectionData.Coordinates.InitialX;
                    int topRightY = (int)clickOnViewXMLSectionData.Coordinates.InitialY;
                    int doubleClickDelay = (int)clickOnViewXMLSectionData.DoubleClickDelay;
                    int afterClickDelay = (int)clickOnViewXMLSectionData.SectionConfig["AfterClickDelay"]["Halt"];

                    Thread.Sleep(initialThreadSleepDuration);

                    Cursor.Position = new Point(topRightX, topRightY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);

                    Thread.Sleep(doubleClickDelay);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);

                    Thread.Sleep(afterClickDelay);
                }
                else
                {
                    _log.Info("ClickOnViewXML configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void ClickOnDownloadXML(JObject config)
        {
            try
            {
                var downloadXMLSectionData = _configReaderService.GetSectionData("ClickOnDownloadXML", config, true, true, false);

                if (downloadXMLSectionData.SectionConfig != null && downloadXMLSectionData != null)
                {
                    int initialThreadSleepDuration = (int)downloadXMLSectionData.InitialDelay;
                    int topRightX = (int)downloadXMLSectionData.Coordinates.InitialX;
                    int topRightY = (int)downloadXMLSectionData.Coordinates.InitialY;
                    int doubleClickDelay = (int)downloadXMLSectionData.DoubleClickDelay;
                    int afterClickDelay = (int)downloadXMLSectionData.SectionConfig["AfterClickDelay"]["Halt"];

                    Thread.Sleep(initialThreadSleepDuration);

                    Cursor.Position = new Point(topRightX, topRightY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);

                    Thread.Sleep(doubleClickDelay);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, topRightX, topRightY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, topRightX, topRightY, 0, 0);

                    Thread.Sleep(afterClickDelay);
                }
                else
                {
                    _log.Info("ClickOnDownloadXML configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void SimulateCopyEvent(JObject config)
        {
            try
            {
                var simulateCopyEventSectionData = _configReaderService.GetSectionData("SimulateCopyEvent", config, false, false, false);

                if (simulateCopyEventSectionData != null)
                {
                    int initialThreadSleepDuration = (int)simulateCopyEventSectionData.InitialDelay;
                    Thread.Sleep(initialThreadSleepDuration);

                    SendKeys.SendWait("^(c)");
                    SendKeys.SendWait("^(c)");
                }
                else
                {
                    _log.Info("SimulateCopyEvent configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }

        // Retrieve data from clipboard
        private string GetDataFromClipboard(JObject config)
        {
            try
            {
                var getDataFromClipboardSectionData = _configReaderService.GetSectionData("GetDataFromClipboard", config, false, false, false);

                if (getDataFromClipboardSectionData != null)
                {
                    int initialThreadSleepDuration = (int)getDataFromClipboardSectionData.InitialDelay;

                    Thread.Sleep(initialThreadSleepDuration);

                    if (Clipboard.ContainsText())
                    {
                        return Clipboard.GetText();
                    }
                    else
                    {
                        _log.Info("No Data found in clipboard");
                        // Handle if no text is found in the clipboard
                        return string.Empty;
                    }
                }
                else
                {
                    _log.Info("GetDataFromClipboard configuration not found in the JSON.");
                    return string.Empty;
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
                return string.Empty;
            }
        }
        private void CopyFolderPath(JObject config)
        {
            try
            {
                var copyFolderPathSectionData = _configReaderService.GetSectionData("CopyFolderPath", config, true, false, false);
                if (copyFolderPathSectionData.SectionConfig != null)
                {
                    int initialThreadSleepDuration = (int)copyFolderPathSectionData.InitialDelay;
                    int initialX = (int)copyFolderPathSectionData.Coordinates.InitialX;
                    int initialY = (int)copyFolderPathSectionData.Coordinates.InitialY;
                    Thread.Sleep(initialThreadSleepDuration);
                    // Move the cursor to the top-left corner (slightly offset to prevent covering the corner pixel)
                    Cursor.Position = new Point(initialX, initialY);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, (int)(uint)initialX, (int)(uint)initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, (int)(uint)initialX, (int)(uint)initialY, 0, 0);
                    SimulateCopyEvent(config);
                }
                else
                {
                    _log.Info("CopyFolderPath configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private void SaveXMLFile(JObject config)
        {
            try
            {
                var saveXMLFileSectionData = _configReaderService.GetSectionData("SaveXMLFile", config, true, false, false);

                if (saveXMLFileSectionData != null)
                {
                    int initialThreadSleepDuration = (int)saveXMLFileSectionData.InitialDelay;
                    int initialX = (int)saveXMLFileSectionData.Coordinates.InitialX;
                    int initialY = (int)saveXMLFileSectionData.Coordinates.InitialY;

                    Thread.Sleep(initialThreadSleepDuration);
                    Cursor.Position = new Point(initialX, initialY);

                    var additionalActions = saveXMLFileSectionData.SectionConfig["AdditionalActions"];

                    var leftMouseDownAction = additionalActions
                        .FirstOrDefault(action => (string)action["Description"] == "MOUSEEVENTF_LEFTDOWN");

                    var leftMouseUpAction = additionalActions
                        .FirstOrDefault(action => (string)action["Description"] == "MOUSEEVENTF_LEFTUP");

                    if (leftMouseDownAction != null && leftMouseUpAction != null)
                    {
                        int leftDownX = (int)leftMouseDownAction["InitialX"];
                        int leftDownY = (int)leftMouseDownAction["InitialY"];
                        int leftUpX = (int)leftMouseUpAction["InitialX"];
                        int leftUpY = (int)leftMouseUpAction["InitialY"];

                        mouse_event(MOUSEEVENTF_LEFTDOWN, leftDownX, leftDownY, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, leftUpX, leftUpY, 0, 0);
                    }
                }
                else
                {
                    _log.Info("SaveXMLFile configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }

        private CptIcdAndDOb GetCptAndIcdCodesAndDOBByXML(string fileAbsolutePath, JObject config)
        {
            CptIcdAndDOb codes = new CptIcdAndDOb
            {
                CPT = new List<CPT>(),
                Dignoses = new List<Dignosis>(),
                PatientPersonalInfo = new PatientPersonalInfo(),
                Encounter = new Encounter()
            };

            try
            {
                var getCptAndIcdCodesAndDOBByXMLSectionDatas = _configReaderService.GetSectionData("GetCptAndIcdCodesAndDOBByXML", config, false, false, false);
                if (getCptAndIcdCodesAndDOBByXMLSectionDatas != null)
                {
                    int initialThreadSleepDuration = (int)getCptAndIcdCodesAndDOBByXMLSectionDatas.InitialDelay;

                    Thread.Sleep(initialThreadSleepDuration);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(fileAbsolutePath); // Replace with your XML file path

                    XmlNode personDobNode = xmlDoc.SelectSingleNode("//Person/BirthDate");

                    if (!string.IsNullOrEmpty(personDobNode?.InnerText))
                    {
                        codes.PatientPersonalInfo.DOB = personDobNode?.InnerText;
                    }

                    XmlNode personNodeID = xmlDoc.SelectSingleNode("//Person/ID");

                    if (!string.IsNullOrEmpty(personNodeID?.InnerText))
                    {
                        string Id = personNodeID?.InnerText;
                        codes.PatientPersonalInfo.Id = Id.Split('-')[0];
                    }

                    // Fetch Diagnosis nodes
                    XmlNodeList diagnosisNodes = xmlDoc.SelectNodes("//Diagnosis");

                    if (diagnosisNodes.Count > 0)
                    {
                        foreach (XmlNode diagnosisNode in diagnosisNodes)
                        {
                            Dignosis diagnosis = new Dignosis
                            {
                                ICDType = diagnosisNode.SelectSingleNode("Type")?.InnerText == "Principal" ? "Primary" : diagnosisNode.SelectSingleNode("Type")?.InnerText,
                                ICDCode = diagnosisNode.SelectSingleNode("Code")?.InnerText
                            };

                            codes.Dignoses.Add(diagnosis);
                        }
                    }

                    // Fetch Encounter start and end dates
                    XmlNode encounterNode = xmlDoc.SelectSingleNode("//Encounter");
                    if (encounterNode != null)
                    {
                        string start = encounterNode.SelectSingleNode("Start")?.InnerText;
                        string end = encounterNode.SelectSingleNode("End")?.InnerText;

                        // Include start and end dates in the result
                        codes.Encounter.StartDate = start;
                        codes.Encounter.EndDate = end;
                    }

                    XmlNodeList activityNodes = xmlDoc.SelectNodes("//Activity");
                    if (activityNodes.Count > 0)
                    {
                        foreach (XmlNode activityNode in activityNodes)
                        {
                            XmlNode codeNode = activityNode.SelectSingleNode("Code");
                            XmlNode quantityNode = activityNode.SelectSingleNode("Quantity");
                            XmlNode type = activityNode.SelectSingleNode("CodeTerm");

                            if (codeNode != null && quantityNode != null && type != null)
                            {
                                string code = codeNode.InnerText;
                                int quantity = int.Parse(quantityNode.InnerText); // Assuming Quantity is an integer
                                string CustHISServiceType = type.InnerText; // Assuming Quantity is an integer

                                if (CustHISServiceType == "CPT4v2012")
                                {
                                    CustHISServiceType = "Laboratory";
                                }
                                if (CustHISServiceType == "CPT_2018")
                                {
                                    CustHISServiceType = "Radiology";
                                }

                                // Assuming you've initialized 'codes.CPT' elsewhere
                                codes.CPT.Add(new CPT { CPTCode = code, Quantity = quantity, Type = CustHISServiceType });
                            }
                        }
                    }
                }
                else
                {
                    _log.Info("GetCptAndIcdCodesAndDOBByXML configuration not found in the JSON.");
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
            return codes;
        }
        //private PatientInfoDto GetWholeSummaryFileData(JObject config)
        //{
        //    try
        //    {
        //        var summaryConfig = config["ReadingSection"].FirstOrDefault(x => (string)x["Section"] == "Summary");

        //        if (summaryConfig != null)
        //        {
        //            int initialThreadSleeper = (int)summaryConfig["InitialThreadSleeper"]["Halt"];
        //            int initialX = (int)summaryConfig["Coordinates"]["InitialX"];
        //            int initialY = (int)summaryConfig["Coordinates"]["InitialY"];
        //            int copyOptionX = (int)summaryConfig["CopyCoordinates"]["CopyOptionX"];
        //            int copyOptionY = (int)summaryConfig["CopyCoordinates"]["CopyOptionY"];
        //            int afterCopyThreadSleeperDuration = (int)summaryConfig["AfterCopyThreadSleeper"]["Halt"];
        //            JArray inBetweenActions = (JArray)summaryConfig["InBetweenActions"];

        //            var inBetweenActionDuration = inBetweenActions
        //                .Select(action => new
        //                {
        //                    Description = (string)action["Description"],
        //                    Duration = (int)action["Duration"]
        //                });

        //            var afterLeftMouseClick = inBetweenActionDuration.FirstOrDefault(action => action.Description == "Left Mouse Clicks");
        //            var afterRightMouseClick = inBetweenActionDuration.FirstOrDefault(action => action.Description == "Right Mouse Clicks");

        //            Thread.Sleep(initialThreadSleeper);

        //            Cursor.Position = new Point(initialX, initialY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

        //            if (afterLeftMouseClick != null)
        //                Thread.Sleep(afterLeftMouseClick.Duration);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
        //            SendKeys.SendWait("^{a}"); // Simulate Ctrl+A (select all)

        //            mouse_event(MOUSEEVENTF_RIGHTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_RIGHTUP, initialX, initialY, 0, 0);

        //            if (afterRightMouseClick != null)
        //                Thread.Sleep(afterRightMouseClick.Duration);

        //            Cursor.Position = new Point(copyOptionX, copyOptionY);
        //            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, copyOptionX, copyOptionY, 0, 0);

        //            Thread.Sleep(afterCopyThreadSleeperDuration);

        //            if (Clipboard.ContainsText())
        //            {
        //                string clipBoardText = Clipboard.GetText();

        //                // Extracting Patient Complaints
        //                string patientComplaintsSection = StringExtensions.GetStringBetween(clipBoardText, "Subjective Notes (History)\r\n\r\nPatient Complaints :\r\n\r\n", "\r\n\r\nVulnerability");
        //                string[] patientComplaints = patientComplaintsSection.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        //                // Fetching Gender
        //                string genderSection = StringExtensions.GetStringBetween(clipBoardText, "Gender\t: ", "\r\nMPI");
        //                string gender = genderSection.Trim();

        //                return new PatientInfoDto
        //                {
        //                    Gender = gender,
        //                    PatientComplaints = string.Join(", ", patientComplaints)
        //                };
        //            }
        //            else
        //            {
        //                _log.Info("No text found in clipboard when tried to fetch summary text");
        //            }
        //        }
        //        else
        //        {
        //            _log.Info("Summary configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //    return new PatientInfoDto();
        //}
        //private void ClickOnPrintPreview(JObject config)
        //{
        //    try
        //    {
        //        var readingSection = config["ReadingSection"];
        //        var clickOnPrintPreviewConfig = readingSection.FirstOrDefault(x => (string)x["Section"] == "ClickOnPrintPreview");

        //        if (clickOnPrintPreviewConfig != null)
        //        {
        //            int initialX = (int)clickOnPrintPreviewConfig["Coordinates"]["InitialX"];
        //            int initialY = (int)clickOnPrintPreviewConfig["Coordinates"]["InitialY"];
        //            int dragDistance = (int)clickOnPrintPreviewConfig["Drag"]["Distance"];
        //            int speedOfDrag = (int)clickOnPrintPreviewConfig["Drag"]["SpeedOfDrag"];
        //            int loopIncrementBy = (int)clickOnPrintPreviewConfig["Drag"]["loopIncrementBy"];
        //            int initialThreadSleeper = (int)clickOnPrintPreviewConfig["InitialThreadSleeper"]["Halt"];

        //            Thread.Sleep(initialThreadSleeper);

        //            Cursor.Position = new Point(initialX, initialY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

        //            var finalPosition = Cursor.Position;
        //            for (int i = 0; i < dragDistance; i += loopIncrementBy)
        //            {
        //                Cursor.Position = new Point(initialX, initialY + i);
        //                Thread.Sleep(speedOfDrag);
        //                finalPosition = Cursor.Position;
        //            }

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, finalPosition.X, finalPosition.Y, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, finalPosition.X, finalPosition.Y, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("ClickOnPrintPreview configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(ex);
        //    }
        //}

        //private void ClosePrintPreview(JObject config)
        //{
        //    try
        //    {
        //        var closePrintPreviewSectionData = _configReaderService.GetSectionData("ClosePrintPreview", config, true, false, true,false);

        //        if (closePrintPreviewSectionData != null)
        //        {
        //            int initialX = (int)closePrintPreviewSectionData.Coordinates.InitialX;
        //            int initialY = (int)closePrintPreviewSectionData.Coordinates.InitialY;
        //            int initialThreadSleepDuration = (int)closePrintPreviewSectionData.InitialDelay;
        //            int beforeDoubleClickThreadSleepDuration = (int)closePrintPreviewSectionData.DoubleClickDelay;

        //            Thread.Sleep(initialThreadSleepDuration);

        //            Cursor.Position = new Point(initialX, initialY);

        //            mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
        //            mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

        //            Thread.Sleep(beforeDoubleClickThreadSleepDuration);

        //            mouse_event(MOUSEEVENTF_DOUBLECLICK, initialX, initialY, 0, 0);
        //        }
        //        else
        //        {
        //            _log.Info("ClosePrintPreview configuration not found in the JSON.");
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error(x);
        //    }
        //}

        private void CloseFollowUpForSelfPayer(JObject config)
        {
            try
            {
                var copyPayerTypeForInsuranceSectionData = _configReaderService.GetSectionData("CopyPayerTypeForInsurance", config, true, false, false);

                if (copyPayerTypeForInsuranceSectionData != null)
                {
                    int initialX = (int)copyPayerTypeForInsuranceSectionData.Coordinates.InitialX;
                    int initialY = (int)copyPayerTypeForInsuranceSectionData.Coordinates.InitialY;

                    CloseFollowUpVisitForInsurance(config);


                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private bool IsInsuranceTypeCustomer(string payerType)
        {
            if (payerType.Equals("Self Pay"))
            {
                // Alternatively, to make it a top-level window across all screens:
                MessageBox.Show(this, "You cannot validate self pay type patients data", "Octopus", MessageBoxButtons.OK);
                if (_appSettings.Configurations.IsNotificationEnabled)
                {
                    _helperService.ShowNotification("You cannot validate self pay type patients data");
                }
                return false;
            }
            else if (payerType.Equals("Insurance"))
            {
                //execute octopus logic for showing validation rules suggestions
                return true;
            }
            else
            {
                return false;
            }
        }
        private string BookAppointment(JObject config)
        {
            // Clear the clipboard
            ClipboardManager.Clear();

            try
            {
                var bookAppointmentSectionData = _configReaderService.GetSectionData("BookAppointment", config, true, false, false);

                if (bookAppointmentSectionData != null)
                {
                    int initialX = (int)bookAppointmentSectionData.Coordinates.InitialX;
                    int initialY = (int)bookAppointmentSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)bookAppointmentSectionData.InitialDelay;
                    Thread.Sleep(initialThreadSleepDuration);

                    SelectOrder(config);

                    Cursor.Position = new Point(initialX, initialY);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                    return CopyPayerTypeForInsurance(config);
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
            return string.Empty;
        }
        private void SelectOrder(JObject config)
        {
            try
            {
                var selectOrderSectionData = _configReaderService.GetSectionData("SelectOrder", config, true, false, false);

                if (selectOrderSectionData != null)
                {
                    int initialX = (int)selectOrderSectionData.Coordinates.InitialX;
                    int initialY = (int)selectOrderSectionData.Coordinates.InitialY;

                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        private string CopyPayerTypeForInsurance(JObject config)
        {

            // Clear the clipboard
            ClipboardManager.Clear();

            try
            {
                var copyPayerTypeForInsuranceSectionData = _configReaderService.GetSectionData("CopyPayerTypeForInsurance", config, true, false, false);

                if (copyPayerTypeForInsuranceSectionData != null)
                {
                    int initialX = (int)copyPayerTypeForInsuranceSectionData.Coordinates.InitialX;
                    int initialY = (int)copyPayerTypeForInsuranceSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)copyPayerTypeForInsuranceSectionData.InitialDelay;
                    int ctrlADelay = (int)copyPayerTypeForInsuranceSectionData.SectionConfig["CtrlADelay"]["Halt"];
                    int afterCopyDelay = (int)copyPayerTypeForInsuranceSectionData.SectionConfig["AfterCopyDelay"]["Halt"];

                    Thread.Sleep(initialThreadSleepDuration);

                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

                    SendKeys.SendWait("^{a}"); // Simulate Ctrl+C (copy)
                    Thread.Sleep(ctrlADelay);
                    SendKeys.SendWait("^{c}"); // Simulate Ctrl+C (copy)
                    Thread.Sleep(afterCopyDelay);

                    CloseFollowUpVisitForInsurance(config);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);

                    return ClipboardManager.GetClipBoardContent();

                }
                else
                {
                    _log.Info("ClickOnOrderBasket configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
            return string.Empty;

        }
        private void CloseFollowUpVisitForInsurance(JObject config)
        {
            try
            {
                var closeFollowUpVisitForInsuranceSectionData = _configReaderService.GetSectionData("CloseFollowUpVisitForInsurance", config, true, false, false);

                if (closeFollowUpVisitForInsuranceSectionData != null)
                {
                    int initialX = (int)closeFollowUpVisitForInsuranceSectionData.Coordinates.InitialX;
                    int initialY = (int)closeFollowUpVisitForInsuranceSectionData.Coordinates.InitialY;
                    //int initialThreadSleepDuration = (int)orderBasketSectionData.InitialDelay;

                    Cursor.Position = new Point(initialX, initialY);
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private void ClickOnOtherActions(JObject config)
        {
            try
            {
                var clickOnOtherActionsSectionData = _configReaderService.GetSectionData("ClickOnOtherActions", config, true, false, false);

                if (clickOnOtherActionsSectionData != null)
                {
                    int initialX = (int)clickOnOtherActionsSectionData.Coordinates.InitialX;
                    int initialY = (int)clickOnOtherActionsSectionData.Coordinates.InitialY;
                    int initialThreadSleepDuration = (int)clickOnOtherActionsSectionData.InitialDelay;

                    Thread.Sleep(initialThreadSleepDuration);

                    Cursor.Position = new Point(initialX, initialY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
                else
                {
                    _log.Info("ClickOnOrderBasket configuration not found in the JSON.");
                }
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }

        }
        private void ClickOnFollowUp(JObject config)
        {
            try
            {
                var clickOnFollowUpSectionData = _configReaderService.GetSectionData("ClickOnFollowUp", config, true, false, false);

                if (clickOnFollowUpSectionData != null)
                {
                    int initialX = (int)clickOnFollowUpSectionData.Coordinates.InitialX;
                    int initialY = (int)clickOnFollowUpSectionData.Coordinates.InitialY;

                    Cursor.Position = new Point(initialX, initialY);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }
        private void CopyPayerTypeForSelfPayer(JObject config)
        {
            try
            {
                var copyPayerTypeForSelfPayerSectionData = _configReaderService.GetSectionData("CopyPayerTypeForSelfPayer", config, true, false, true);

                if (copyPayerTypeForSelfPayerSectionData != null)
                {
                    int initialX = (int)copyPayerTypeForSelfPayerSectionData.Coordinates.InitialX;
                    int initialY = (int)copyPayerTypeForSelfPayerSectionData.Coordinates.InitialY;
                    int contextMenuX = (int)copyPayerTypeForSelfPayerSectionData.contextMenuActions.InitialX;
                    int contextMenuY = (int)copyPayerTypeForSelfPayerSectionData.contextMenuActions.InitialY;
                    int ctrlADelay = (int)copyPayerTypeForSelfPayerSectionData.SectionConfig["CtrlADelay"]["Halt"];
                    int afterCopyDelay = (int)copyPayerTypeForSelfPayerSectionData.SectionConfig["AfterCopyDelay"]["Halt"];

                    // Move cursor to the left
                    Cursor.Position = new Point(initialX, initialY);

                    //// Perform another click to the left
                    mouse_event(MOUSEEVENTF_LEFTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, initialX, initialY, 0, 0);
                    // ... (your existing code)

                    SendKeys.SendWait("^{a}"); // Simulate Ctrl+A (select all)
                    SendKeys.SendWait("^{a}"); // Simulate Ctrl+A (select all)
                    Thread.Sleep(ctrlADelay);

                    // Perform a right-click at the specified coordinates
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, initialX, initialY, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, initialX, initialY, 0, 0);
                    Thread.Sleep(ctrlADelay);

                    // Move the cursor to the "Copy" option
                    Cursor.Position = new Point(contextMenuX, contextMenuY);
                    // Simulate a left-click to select the "Copy" option from the context menu
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, contextMenuX, contextMenuY, 0, 0);

                    SendKeys.SendWait("^{c}"); // Simulate Ctrl+A (select all)

                    Thread.Sleep(afterCopyDelay);
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }

        private void myProfile_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
            CloseChildFloatingIcons();
            iconForm.Close();
        }
        private string FetchPatientMPI()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("FetchPatientMPI", _config, true, true, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
                int doubleClickDelay = (int)closeDiagnosisTabSectionData.DoubleClickDelay;
                int retrieveDelay = (int)closeDiagnosisTabSectionData.SectionConfig["RetrieveDataFromClipboard"]["RetrieveDelay"]["Halt"];

                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                Thread.Sleep(doubleClickDelay);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

                // Simulate copy (Ctrl+C)
                SendKeys.SendWait("^(c)");
                Thread.Sleep(retrieveDelay);

                /// Retrieve copied content from the clipboard
                string copiedText = Clipboard.GetText();

                // Now 'copiedText' contains the content copied from the specified coordinates
                return copiedText;
            }
            return string.Empty;
        }
    }
}
