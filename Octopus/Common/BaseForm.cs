using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Dtos.AppSettingDtos;
using Octopus.Dtos.Common;
using Octopus.Helpers.CustomHelpers;
using Octopus.Helpers.CustomHelpers.Authentication.Login.Alerts;
using Octopus.Models.Singleton.DignosisGridDtos;
using Octopus.Models.Singleton.PatientConditionsDtos;
using Octopus.Models.Singleton.PatientMpiDtos;
using Octopus.Models.Singleton.PatientMpiDtos.Octopus.Models.Singleton.PatientMpiDtos;
using Octopus.Models.Singleton.ServicesGridDtos;
using Octopus.Services.AuthenticationServices;
using Octopus.Services.ConfigReaderServices;
using Tesseract;
using WindowsInput;

namespace Octopus.Common
{
    public class LazyWrapper<T>
    {
        private readonly Func<T> _valueFactory;
        private Lazy<T> _lazy;

        public T Value
        {
            get
            {
                if (_lazy == null || (_lazy.IsValueCreated && EqualityComparer<T>.Default.Equals(_lazy.Value, default(T))))
                {
                    _lazy = new Lazy<T>(_valueFactory);
                }

                if (_lazy.Value == null)
                {
                    _lazy = new Lazy<T>(_valueFactory); // If the value is null, refresh the lazy initialization
                }

                return _lazy.Value;
            }
        }

        public LazyWrapper(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public void ForceRefresh(Func<T> valueFactory)
        {
            _lazy = new Lazy<T>(valueFactory); // Recreate the LazyWrapper with the updated value
        }

    }

    public partial class BaseForm : Form
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117
        }


        public static double GetWindowsScreenScalingFactor(bool percentage = true)
        {
            //Create Graphics object from the current windows handle
            Graphics GraphicsObject = Graphics.FromHwnd(IntPtr.Zero);
            //Get Handle to the device context associated with this Graphics object
            IntPtr DeviceContextHandle = GraphicsObject.GetHdc();
            //Call GetDeviceCaps with the Handle to retrieve the Screen Height
            int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
            //Divide the Screen Heights to get the scaling factor and round it to two decimals
            double ScreenScalingFactor = Math.Round(PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
            //If requested as percentage - convert it
            if (percentage)
            {
                ScreenScalingFactor *= 100.0;
            }
            //Release the Handle and Dispose of the GraphicsObject object
            GraphicsObject.ReleaseHdc(DeviceContextHandle);
            GraphicsObject.Dispose();
            //Return the Scaling Factor
            return ScreenScalingFactor;
        }

        public static Size GetDisplayResolution()
        {
            var sf = GetWindowsScreenScalingFactor(false);
            var screenWidth = Screen.PrimaryScreen.Bounds.Width * sf;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height * sf;
            return new Size((int)screenWidth, (int)screenHeight);
        }


        //mouse scroll
        const int MOUSEEVENTF_WHEEL = 0x0800;

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        //mouse scroll
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
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(BaseForm));

        internal IExtension _helperService;
        internal AppSettingsDTO _appSettings = new AppSettingsDTO();
        internal JObject _config = new JObject();
        internal readonly IConfigReaderService _configReaderService;
        private readonly (int Width, int Height) _myDeviceResolution;

        private readonly Size _currentDisplayResolution;

        public Size CurrentDisplayResolution
        {
            get { return _currentDisplayResolution; }
        }

        // Private readonly property to expose the current working directory within the class
        private readonly string _currentWorkingDirectory;
        // Expose the behavior of GetCurrentWorkingDirectory through a readonly property for other classes in this assembly
        internal string CurrentWorkingDirectory => _currentWorkingDirectory;
        internal string _configFile { get; set; }

        private readonly string _appSettingsFileName = "appsettings.json";

        internal IAuthenticationService _authenticationService;

        // Lazy initialization for the auth token
        private /*readonly*/ LazyWrapper<string> _authToken;

        // Expose public readonly property for AuthToken
        internal string AuthToken => _authToken.Value;

        // Encapsulated readonly property for AuthToken
        private string PreviousToken
        {
            get
            {
                var authToken = Properties.Settings.Default.authToken;
                return string.IsNullOrEmpty(authToken) ? null : authToken;
            }
        }
        public BaseForm()
        {
            InitializeComponent();
            _currentDisplayResolution = GetDisplayResolution();
            _helperService = new Extension();
            _currentWorkingDirectory = _helperService.GetCurrentWorkingDirectory();
            _appSettings = ReadAppSettings();
            SetConfigFileName(_appSettings?.DeviceConfigurations?.ConfigurationFileName);
            _config = FetchConfigFileData();
            _configReaderService = new ConfigReaderService();
            _myDeviceResolution = AppSettingManager.GetCurrentResolution(_appSettings);

            // Check if AuthToken is null before deciding to use lazy initialization
            if (PreviousToken == null)
            {
                // Initialize AuthenticationService with necessary parameters
                _authenticationService = new AuthenticationService(_apiBaseURl, _authUserId, _authLoginPassword);

                // Lazy initialization of _authToken
                _authToken = new LazyWrapper<string>(() => _authenticationService.GetAuthToken());

                // Subscribe to the AuthTokenRetrieved event
                _authenticationService.AuthTokenRetrieved += OnAuthTokenRetrieved;
            }
            else
            {
                // Fetch the token from app settings
                _authToken = new LazyWrapper<string>(() => PreviousToken);
            }
        }
        //internal string _configFile { get { return Properties.Settings.Default.ConfigurationFileName; } }
        //internal static string _authToken { get { return new BaseForm().fetchAuthToken(); } }
        internal static string _apiBaseURl { get { return Properties.Settings.Default.apiBaseUrl; } }
        internal string _authLoginPassword { get { return Properties.Settings.Default.appPassword; } }
        internal string _authUserId { get { return Properties.Settings.Default.appUserName; } }
        private void BaseForm_Load(object sender, EventArgs e)
        {

        }

        #region Common

        private void OnAuthTokenRetrieved(string authToken)
        {
            // Handle the event, if needed
            if (authToken == Alert.UsernamePasswordIsRequired)
            {
                _log.Error("Please set the auth credentials in app settings before moving forward");
            }
            else if (authToken == Alert.LoginFailedMessage)
            {
                _log.Info("You have entered Invalid incredentials while trying to acquire auth token");
            }
            else
            {
                // Save auth token to app settings for later use
                AppSettingManager.Save("authToken", authToken);
            }
        }
        // Method to set the _authToken value from the derived form (e.g., Login form)
        internal void RefreshAuthToken(Func<string> authTokenRetrievalFunction)
        {
            _authToken.ForceRefresh(authTokenRetrievalFunction);
        }
        internal void OnAuthTokenRefreshed(string authToken)
        {
            // Save auth token to app settings for later use
            AppSettingManager.Save("authToken", authToken);
        }
        //private string fetchAuthToken()
        //{
        //    var authToken = Properties.Settings.Default.authToken;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(authToken))
        //        {
        //            if (!string.IsNullOrEmpty(_authUserId) && !string.IsNullOrEmpty(_authLoginPassword))
        //            {

        //                //make api call in order to fetch auth Token
        //                authToken = AuthenticationService.Authenticate(new AuthenticationDto
        //                {
        //                    UserId = _authUserId,
        //                    Password = _authLoginPassword
        //                }
        //                , _apiBaseURl);

        //                //save auth token to app settings for later use
        //                AppSettingManager.Save("authToken", authToken);

        //                return authToken;
        //            }
        //            else
        //            {
        //                _log.Error("Please set the auth credentials in app settings before moving forward");
        //            }
        //        }
        //        else
        //        {
        //            return authToken;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
        //        _log.Error("Error Message: " + ex.Message.ToString(), ex);
        //    }
        //    return authToken;
        //}
        // Event handler for AuthTokenRetrieved event
        internal bool isCurrentScreenLTDoctorsDesk()
        {
            try
            {
                // Capture the screen
                Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, screenshot.Size);
                }

                // Save the screenshot (optional)
                screenshot.Save("screenshot.png", System.Drawing.Imaging.ImageFormat.Png);

                string currentDirectory = _currentWorkingDirectory;

                // Initialize Tesseract engine
                using (var engine = new TesseractEngine($"{currentDirectory}\\tessdata", "eng", EngineMode.Default))
                {
                    // Set the path to the tessdata directory containing language data (replace "path\to\tessdata" with the actual path)
                    using (var img = Pix.LoadFromFile("screenshot.png"))
                    {
                        using (var page = engine.Process(img))
                        {
                            string extractedText = page.GetText();

                            // Texts to search for
                            string[] textsToSearch = _appSettings.TextsToSearch;
                            //string[] textsToSearch = { "VLifetrenz", "History", "Examination (Objective)", "Order", "Diagnosis (Assessment)" };

                            // Condition to check if all texts exist in the extracted text
                            bool allTextsExist = textsToSearch.All(text => extractedText.Contains(text));


                            return allTextsExist;

                            //// Condition to check if any text exists in the extracted text
                            //bool anyTextExists = textsToSearch.Any(text => extractedText.Contains(text));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
            return false;
        }
        private AppSettingsDTO ReadAppSettings()
        {
            string currentDirectory = _currentWorkingDirectory;
            string fileName = _appSettingsFileName; // Your file name

            string filePath = Path.Combine(currentDirectory, fileName);
            AppSettingsDTO appSettings = new AppSettingsDTO();
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader r = new StreamReader(filePath))
                    {
                        string json = r.ReadToEnd();
                        appSettings = JsonConvert.DeserializeObject<AppSettingsDTO>(json);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error reading the JSON file: " + e.Message);
                }
            }
            return appSettings;
        }
        internal void ConvertResolution()
        {
            try
            {
                // Fetch my device resolution
                var resolution = _myDeviceResolution;

                var sizeOfScreen = CurrentDisplayResolution;

                if (sizeOfScreen.Width.Equals(resolution.Width) && sizeOfScreen.Height.Equals(resolution.Height))
                {
                    return;
                }

                // Fetching coordinates from JSON metadata config from global variable
                JObject jsonObject = _config;

                // Perform coordinate conversion
                jsonObject.ConvertCoordinatesToNewResolution(sizeOfScreen.Width, sizeOfScreen.Height, resolution);

                // Save updated JSON to a new file
                string updatedJsonContent = jsonObject.ToString(Formatting.Indented);

                string newFileName = $"octopus.config-{sizeOfScreen.Width}-{sizeOfScreen.Height}.json";

                string newFilePath = Path.Combine(_currentWorkingDirectory, newFileName);

                File.WriteAllText(newFilePath, updatedJsonContent);

                //save config filename to app settings for later use
                SaveAppSettings("ConfigurationFileName", newFileName);

                //save updated screen resolution default value in app settings for later use
                SaveUpdatedResolution("CurrentDeviceResolution", sizeOfScreen.Width, sizeOfScreen.Height);

                //reinitialize appsettings instance
                _appSettings = new AppSettingsDTO();

                //reload appsettings
                _appSettings = ReadAppSettings();

                //reload config file name
                SetConfigFileName(_appSettings?.DeviceConfigurations?.ConfigurationFileName);

                //reinitialize _config instance 
                _config = new JObject();

                //reload newly created config file data into _config variable
                _config = FetchConfigFileData();
            }
            catch (Exception x)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error(x);
            }
        }
        //internal Image FetchIcon(string iconName)
        //{
        //    string currentDirectory = _currentWorkingDirectory;
        //    if (iconName == "Minimize")
        //    {
        //        // Create the icon PictureBox based on the service type
        //        string iconPath = "icons8-cancel-64.png";
        //        // Load the icon image based on the iteration
        //        string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", iconPath);
        //        Image iconImage = _helperService.LoadImage(iconImagePath);
        //        return iconImage;
        //    }
        //    else
        //    {
        //        // Create the icon PictureBox based on the service type
        //        string iconPath = "icons8-close-window-94.png";
        //        // Load the icon image based on the iteration
        //        string iconImagePath = Path.Combine(currentDirectory, "Resources", "Assets", "Icon", iconPath);
        //        Image iconImage = _helperService.LoadImage(iconImagePath);
        //        return iconImage;
        //    }
        //}
        static internal void AddToDignosisGridSingelton(List<DiagnosisGridDto> data)
        {
            // 1. Adding data to the shared instance
            DiagnosisGridDtoList dtoList = DiagnosisGridDtoList.GetInstance();

            //clear list
            dtoList.ClearDiagnosisList();

            //save in this singleton instance
            dtoList.AddDiagnosisList(data);
        }
        internal static void AddDataToServiceGridSingelton(List<ServicesGridDto> data)
        {
            // 1. Adding data to the shared instance
            ServiceGridDtoList dtoList = ServiceGridDtoList.GetInstance();

            //clear list
            dtoList.ClearServiceList();

            //save in this singleton instance
            dtoList.AddServiceList(data);
        }
        // Method to add data to the singleton instance
        internal static void AddToPatientConditionsSingleton(PatientConditionsDto data)
        {
            // 1. Adding data to the shared instance
            PatientConditionsDtoObject dtoObject = PatientConditionsDtoObject.GetInstance();

            // Clear previous value
            dtoObject.ClearPatientConditions();

            // Save the new value in this singleton instance
            dtoObject.SetPatientConditions(data);
        }
        // Method to fetch data from the singleton instance
        internal static PatientConditionsDto GetPatientConditionsFromSingleton()
        {
            PatientConditionsDtoObject dtoObject = PatientConditionsDtoObject.GetInstance();
            return dtoObject.GetPatientConditions();
        }
        static internal void AddToPatientMpiSingleton(PatientMpiDto data)
        {
            // 1. Adding data to the shared instance
            PatientMpiDtoObject dtoObject = PatientMpiDtoObject.GetInstance();

            // Clear previous value
            dtoObject.ClearPatientMpi();

            // Save the new value in this singleton instance
            dtoObject.SetPatientMpi(data);
        }
        static internal PatientMpiDto GetStoredPatientMpi()
        {
            // Accessing the shared instance
            PatientMpiDtoObject dtoObject = PatientMpiDtoObject.GetInstance();

            // Retrieve the stored PatientMpiDto
            return dtoObject.GetPatientMpi();
        }
        private JObject FetchConfigFileData()
        {
            try
            {
                string currentDirectory = _currentWorkingDirectory;

                string fileName = _configFile; // Your file name

                string filePath = Path.Combine(currentDirectory, fileName);

                if (File.Exists(filePath))
                {
                    // Read the file content or perform operations here
                    string fileContent = File.ReadAllText(filePath);
                    JObject config = JObject.Parse(fileContent);
                    return config;
                }
                else
                {
                    _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                    _log.Error($"File with name {fileName} not found");
                }
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
            return new JObject();
        }
        //mouse scroll
        internal void MouseScrollUp()
        {
            var scrollMouseSectionData = _configReaderService.GetSectionData("ScrollMouse", _config, true, false, false);

            if (scrollMouseSectionData != null)
            {
                int initialThreadSleepDuration = (int)scrollMouseSectionData.InitialDelay;

                int scrollAmount = (int)scrollMouseSectionData.SectionConfig["ScrollAction"]["ScrollAmount"];
                int numScrolls = (int)scrollMouseSectionData.SectionConfig["ScrollAction"]["NumScrolls"];
                int scrollDelay = (int)scrollMouseSectionData.SectionConfig["ScrollAction"]["ScrollDelay"];

                Thread.Sleep(initialThreadSleepDuration);

                //mouse scroll
                int x = scrollMouseSectionData.Coordinates.InitialX;
                int y = scrollMouseSectionData.Coordinates.InitialY;

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

                for (int i = 0; i < numScrolls; i++)
                {
                    ScrollMouse(scrollAmount);
                    Thread.Sleep(scrollDelay); // Adjust this delay if needed
                }
            }
            //mouse scroll
        }
        static void ScrollMouse(int amount)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, amount, 0);
        }
        static void DragMouseRightToLeft(int distance)
        {
            InputSimulator inputSimulator = new InputSimulator();
            inputSimulator.Mouse.LeftButtonDown();
            inputSimulator.Mouse.MoveMouseBy(distance * -1, 0);
            inputSimulator.Mouse.LeftButtonUp();

            // Optional: Sleep to add a delay between actions
            Thread.Sleep(500);
        }
        static void LeftMouseClick()
        {
            InputSimulator inputSimulator = new InputSimulator();
            inputSimulator.Mouse.LeftButtonClick();
        }
        internal void CollapseExpandedOrderSectionsIfNeeded()
        {
            if (_appSettings.Configurations.IsOrderTabSectionsAutoCollapseEnabled)
            {
                DialogResult result = MessageBox.Show("Are all the order tab sections are expanded?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // User clicked Yes, handle the action here
                    CollapseAllExpandedOrderSections();
                }
                else
                {
                    // User clicked No or closed the dialog, handle it accordingly
                }
            }
        }
        //mouse scroll
        #endregion Common

        #region Dignosis Tab
        //dignosis tab releated methods starts here.
        //these methods below have been used by ICDSuggestionsForm Accept Button & RuleValidation Accept Button 
        private void ClickOnCurrentDignosis()
        {
            var clickOnCurrentDiagnosisSectionData = _configReaderService.GetSectionData("ClickOnCurrentDiagnosis", _config, true, false, false);

            if (clickOnCurrentDiagnosisSectionData != null)
            {
                // Set the coordinates where you want to click
                int x = (int)clickOnCurrentDiagnosisSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnCurrentDiagnosisSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void MoveCursorToSearchBarOfDignosis()
        {
            var moveCursorToSearchBarOfDiagnosisSectionData = _configReaderService.GetSectionData("MoveCursorToSearchBarOfDiagnosis", _config, true, false, false);

            if (moveCursorToSearchBarOfDiagnosisSectionData != null)
            {
                int initialThreadSleepDuration = (int)moveCursorToSearchBarOfDiagnosisSectionData.InitialDelay;
                Thread.Sleep(initialThreadSleepDuration);
                // Set the coordinates where you want to click
                int x = (int)moveCursorToSearchBarOfDiagnosisSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)moveCursorToSearchBarOfDiagnosisSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void SearchDignosis(string icdCode)
        {
            var searchDiagnosisSectionData = _configReaderService.GetSectionData("SearchDiagnosis", _config, false, false, false);

            if (searchDiagnosisSectionData != null)
            {
                int initialThreadSleepDuration = (int)searchDiagnosisSectionData.InitialDelay;

                // Mimic type behaviour inside search box of dignosis
                // Mimic typing "Hello, world!" with delays between each key press
                string textToType = icdCode;

                foreach (char c in textToType)
                {
                    SendKeys.SendWait(c.ToString());
                    Thread.Sleep(initialThreadSleepDuration); // Adjust the delay between each keystroke (in milliseconds)
                }

                // Press Enter key
                SendKeys.SendWait("{ENTER}");
            }
        }
        private void ClickOnRowItemFromDignosisGrid()
        {
            var clickOnRowItemFromGridSectionData = _configReaderService.GetSectionData("ClickOnRowItemFromDignosisGrid", _config, true, true, false);

            if (clickOnRowItemFromGridSectionData != null)
            {
                int initialThreadSleepDuration = (int)clickOnRowItemFromGridSectionData.InitialDelay;
                int doubleClickDelay = (int)clickOnRowItemFromGridSectionData.DoubleClickDelay;
                Thread.Sleep(initialThreadSleepDuration);
                // Set the coordinates where you want to click
                int x = (int)clickOnRowItemFromGridSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnRowItemFromGridSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                Thread.Sleep(doubleClickDelay);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        //private void ClickOnAddDignosisButton()
        //{
        //    var clickOnAddDiagnosisButtonSectionData = _configReaderService.GetSectionData("ClickOnAddDiagnosisButton", _config, true, false, false);

        //    if (clickOnAddDiagnosisButtonSectionData != null)
        //    {
        //        int initialThreadSleepDuration = (int)clickOnAddDiagnosisButtonSectionData.InitialDelay;
        //        Thread.Sleep(initialThreadSleepDuration);
        //        // Set the coordinates where you want to click
        //        int x = (int)clickOnAddDiagnosisButtonSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //        int y = (int)clickOnAddDiagnosisButtonSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //        // Move the mouse cursor to the specific location
        //        Cursor.Position = new System.Drawing.Point(x, y);

        //        // Perform left mouse button down and up actions
        //        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //    }
        //}
        private void CloseDignosisTab()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("CloseDiagnosisTab", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        //dignosis tab releated methods ends here
        #endregion

        #region Dignosis tab releated
        //private void RemoveICDs(int icdsCount)
        //{
        //    MimicRemoveDignosisClick(icdsCount);
        //}
        //private void MimicRemoveDignosisClick(int icdsCount)
        //{

        //    var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("MimicRemoveDiagnosisClick", _config, true, false, false);

        //    if (closeDiagnosisTabSectionData != null)
        //    {
        //        int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;


        //        // Set the coordinates where you want to click
        //        int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //        int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //        // Move the mouse cursor to the specific location
        //        Cursor.Position = new Point(x, y);
        //        for (var i = 0; i < icdsCount; i++)
        //        {
        //            Thread.Sleep(initialThreadSleepDuration);

        //            // Perform left mouse button down and up actions
        //            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //        }
        //    }
        //}
        private void ClickOnDignosisPrimaryRadioBtn()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDignosisPrimaryRadioBtn", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the starting position
                Cursor.Position = new Point(x, y);

                //click on Secondary radio button on dignosis tab
                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                //click on Secondary radio button on dignosis tab
            }
        }
        private void ClickOnDignosisSecondaryRadioBtn()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDignosisSecondaryRadioBtn", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the starting position
                Cursor.Position = new Point(x, y);

                //click on Secondary radio button on dignosis tab
                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                //click on Secondary radio button on dignosis tab
            }
        }
        private void CollapseUnCollapseCurrentDignosis()
        {
            //click on Secondary radio button on dignosis tab
            // Set the coordinates where you want to click
            int x = (int)785; // Replace with your desired X coordinate
            int y = (int)284; // Replace with your desired Y coordinate

            Thread.Sleep(1000);

            // Move the mouse cursor to the starting position
            Cursor.Position = new System.Drawing.Point(x, y);
            // Perform left mouse button down and up actions
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }
        private void ClickOnCurrentMultiaxialDiagnoses()
        {
            //click on Secondary radio button on dignosis tab
            // Set the coordinates where you want to click
            int x = (int)809; // Replace with your desired X coordinate
            int y = (int)370; // Replace with your desired Y coordinate

            Thread.Sleep(1000);

            // Move the mouse cursor to the starting position
            Cursor.Position = new System.Drawing.Point(x, y);
            // Perform left mouse button down and up actions
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }
        private void ClickOnAxisIII()
        {
            //click on Secondary radio button on dignosis tab
            // Set the coordinates where you want to click
            int x = (int)785; // Replace with your desired X coordinate
            int y = (int)195; // Replace with your desired Y coordinate

            Thread.Sleep(1000);

            // Move the mouse cursor to the starting position
            Cursor.Position = new System.Drawing.Point(x, y);
            // Perform left mouse button down and up actions
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            Thread.Sleep(2500);
        }
        private void DeleteICD(int icdsCount)
        {
            //click on Secondary radio button on dignosis tab
            // Set the coordinates where you want to click
            int x = (int)1345; // Replace with your desired X coordinate
            int y = (int)526; // Replace with your desired Y coordinate
            Thread.Sleep(1000);
            // Move the mouse cursor to the starting position
            Cursor.Position = new Point(x, y);
            for (var i = 0; i < icdsCount; i++)
            {
                Thread.Sleep(1000);

                mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            }
        }
        private void CloseCurrentMultiaxialDiagnoses()
        {
            // Set the coordinates where you want to click
            int x = (int)1377; // Replace with your desired X coordinate
            int y = (int)157; // Replace with your desired Y coordinate

            Thread.Sleep(1000);

            // Move the mouse cursor to the starting position
            Cursor.Position = new System.Drawing.Point(x, y);
            // Perform left mouse button down and up actions
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }
        //internal void AddAndDeleteICDinDignosisTab(List<string> icdCodes, int removeRowCount, bool deletionRequired,bool primaryAdditionRequired)
        //{

        //    if (deletionRequired)
        //    {
        //        //mimic delete icds behaviour in dignosis tab in doctors desk in lifetrenz 
        //        CollapseUnCollapseCurrentDignosis();
        //        ClickOnCurrentMultiaxialDiagnoses();
        //        ClickOnAxisIII();
        //        DeleteICD(removeRowCount);
        //        CloseCurrentMultiaxialDiagnoses();
        //        CollapseUnCollapseCurrentDignosis();
        //        //RemoveICDs(removeRowCount);
        //    }

        //    ClickOnCurrentDignosis();

        //    if (primaryAdditionRequired)
        //    {
        //        var icdCode = icdCodes.FirstOrDefault();
        //        ClickOnDignosisPrimaryRadioBtn();

        //        MoveCursorToSearchBarOfDignosis();

        //        // Simulate Ctrl+A (Select All)
        //        SendKeys.SendWait("^a"); // "^" represents the Ctrl key
        //        SendKeys.SendWait("^a"); // "^" represents the Ctrl key

        //        // Simulate Backspace
        //        SendKeys.SendWait("{BACKSPACE}");
        //        SendKeys.SendWait("{BACKSPACE}");

        //        SearchDignosis(icdCode);
        //        ClickOnRowItemFromDignosisGrid();

        //        ClickOnDignosisSecondaryRadioBtn();

        //        foreach (var code in icdCodes.Skip(1))
        //        {
        //            MoveCursorToSearchBarOfDignosis();

        //            // Simulate Ctrl+A (Select All)
        //            SendKeys.SendWait("^a"); // "^" represents the Ctrl key
        //            SendKeys.SendWait("^a"); // "^" represents the Ctrl key

        //            // Simulate Backspace
        //            SendKeys.SendWait("{BACKSPACE}");
        //            SendKeys.SendWait("{BACKSPACE}");

        //            SearchDignosis(code);
        //            ClickOnRowItemFromDignosisGrid();
        //        }
        //    }
        //    else
        //    {
        //        ClickOnDignosisSecondaryRadioBtn();

        //        foreach (var code in icdCodes)
        //        {
        //            MoveCursorToSearchBarOfDignosis();

        //            // Simulate Ctrl+A (Select All)
        //            SendKeys.SendWait("^a"); // "^" represents the Ctrl key
        //            SendKeys.SendWait("^a"); // "^" represents the Ctrl key

        //            // Simulate Backspace
        //            SendKeys.SendWait("{BACKSPACE}");
        //            SendKeys.SendWait("{BACKSPACE}");

        //            SearchDignosis(code);
        //            ClickOnRowItemFromDignosisGrid();
        //        }
        //    }
        //    //close dignosis tab
        //    CloseDignosisTab();
        //}

        internal void AddAndDeleteICDinDignosisTab(List<string> icdCodes, int removeRowCount, bool deletionRequired, bool primaryAdditionRequired)
        {
            if (deletionRequired)
            {
                PerformICDDeletionSteps(removeRowCount);
            }

            ClickOnCurrentDignosis();

            if (primaryAdditionRequired)
            {
                AddPrimaryICD(icdCodes.FirstOrDefault());

                // Skip the first item for secondary addition
                AddSecondaryICDs(icdCodes.Skip(1));
            }
            else
            {
                // Add all the ICDs after clicking the secondary radio button
                ClickOnDignosisSecondaryRadioBtn();
                AddICDs(icdCodes);
            }

            // Close the Dignosis tab
            CloseDignosisTab();
        }

        private void PerformICDDeletionSteps(int removeRowCount)
        {
            // Mimic delete ICDs behavior in Diagnosis tab
            CollapseUnCollapseCurrentDignosis();
            ClickOnCurrentMultiaxialDiagnoses();
            ClickOnAxisIII();
            DeleteICD(removeRowCount);
            CloseCurrentMultiaxialDiagnoses();
            CollapseUnCollapseCurrentDignosis();
        }

        private void AddPrimaryICD(string icdCode)
        {
            ClickOnDignosisPrimaryRadioBtn();
            AddICD(icdCode);
        }
        private void AddICD(string icdCode)
        {
            MoveCursorToSearchBarOfDignosis();
            ClearSearchBar();
            SearchDignosis(icdCode);
            ClickOnRowItemFromDignosisGrid();
        }

        private void AddSecondaryICDs(IEnumerable<string> icdCodes)
        {
            ClickOnDignosisSecondaryRadioBtn();
            AddICDs(icdCodes);
        }

        private void AddICDs(IEnumerable<string> icdCodes)
        {
            foreach (var code in icdCodes)
            {
                MoveCursorToSearchBarOfDignosis();
                ClearSearchBar();
                SearchDignosis(code);
                ClickOnRowItemFromDignosisGrid();
            }
        }

        private void ClearSearchBar()
        {
            // Simulate Ctrl+A (Select All)
            SendKeys.SendWait("^a");
            SendKeys.SendWait("^a");

            // Simulate Backspace
            SendKeys.SendWait("{BACKSPACE}");
            SendKeys.SendWait("{BACKSPACE}");
        }

        #endregion Dignosis tab releated

        #region Radiology Section
        private void ClickOnRadiologyOrders()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyOrders", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate


                // Move the mouse cursor to the specific location
                Cursor.Position = new Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnRadiologyOrderSearchBar()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyOrderSearchBar", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnRadiologyOrderGridRow()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyOrderGridRow", _config, true, true, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
                int doubleClickDelay = (int)closeDiagnosisTabSectionData.DoubleClickDelay;

                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                Thread.Sleep(doubleClickDelay);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        //private void ClickOnRadiologyOrderAddMore()
        //{

        //    var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyOrderAddMore", _config, true, false, false);

        //    if (closeDiagnosisTabSectionData != null)
        //    {
        //        int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
        //        int afterClickDelay = (int)closeDiagnosisTabSectionData.SectionConfig["AfterClickDelay"]["Halt"]; // Replace with your desired Y coordinate

        //        // Set the coordinates where you want to click
        //        int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //        int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //        Thread.Sleep(initialThreadSleepDuration);


        //        // Move the mouse cursor to the specific location
        //        Cursor.Position = new System.Drawing.Point(x, y);

        //        // Perform left mouse button down and up actions
        //        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

        //        Thread.Sleep(afterClickDelay);
        //    }
        //}
        private void CloseRadiologyOrders()
        {

            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("CloseRadiologyOrders", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnRadiologyShowMoreBtn()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyShowMoreBtn", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        #endregion Radiology

        #region Radiology order
        private void ClickOnDeleteRadiology(int cptCount)
        {
            var closeDiagnosisTabSectionDataKey = "ClickOnDeleteRadiologyWithoutShowMore";

            if (cptCount > 5)
            {
                closeDiagnosisTabSectionDataKey = "ClickOnDeleteRadiology";
                ClickOnRadiologyShowMoreBtn();
            }

            ClickOnDeleteCommon(cptCount, closeDiagnosisTabSectionDataKey);
        }
        internal void AddAndDeleteCPTInRadiology(List<string> cptCodes, int removeRowCount, bool deletionRequired)
        {
            ClickOnRadiologyOrders();

            if (deletionRequired)
            {
                //function here for mimicing/simulating the deletion for all cpts in radiology popup
                ClickOnDeleteRadiology(removeRowCount);
            }

            foreach (var code in cptCodes)
            {
                ClickOnRadiologyOrderSearchBar();

                // Simulate Ctrl+A (Select All)
                SendKeys.SendWait("^a"); // "^" represents the Ctrl key
                SendKeys.SendWait("^a"); // "^" represents the Ctrl key

                // Simulate Backspace
                SendKeys.SendWait("{BACKSPACE}");
                SendKeys.SendWait("{BACKSPACE}");
                SearchCPT(code);
                ClickOnRadiologyOrderGridRow();
                //ClickOnRadiologyOrderAddMore();
            }
            CloseRadiologyOrders();
        }
        #endregion Radiology order

        #region Common functions for lab orders and radiology orders
        private void SearchCPT(string text)
        {
            var searchCPTSectionData = _configReaderService.GetSectionData("SearchCPT", _config, false, false, false);

            if (searchCPTSectionData != null)
            {
                int initialThreadSleepDuration = (int)searchCPTSectionData.InitialDelay;
                int keyPressDelay = (int)searchCPTSectionData.SectionConfig["KeyPressDelay"]["Halt"];
                Thread.Sleep(initialThreadSleepDuration);

                // Mimic type behaviour inside search box of dignosis
                // Mimic typing "Hello, world!" with delays between each key press
                string textToType = text;

                foreach (char c in textToType)
                {
                    SendKeys.SendWait(c.ToString());
                    Thread.Sleep(keyPressDelay); // Adjust the delay between each keystroke (in milliseconds)
                }

                // Press Enter key
                SendKeys.SendWait("{ENTER}");
            }
        }
        private void ClickOnDeleteCommon(int count, string sectionKey)
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData(sectionKey, _config, true, true, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
                int doubleClickDelay = (int)closeDiagnosisTabSectionData.DoubleClickDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                for (int i = 0; i < count; i++)
                {
                    // Move the mouse cursor to the specific location
                    Cursor.Position = new Point(x, y);

                    // Perform left mouse button down and up actions
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                    Thread.Sleep(500);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

                    Thread.Sleep(doubleClickDelay);
                }
            }
        }
        #endregion Common functions for lab orders and radiology orders

        #region CollapseExpand order section +,- icon
        private void ClickOnPlanNotesPlusSign()
        {

            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnPlanNotesPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnHomeActiveMedicationsPlusSign()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnHomeActiveMedicationsPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnCPOEPlusSign()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnCPOEPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnOrderSetPlusSign()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnOrderSetPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnMedicationOrdersPlusSign()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnMedicationOrdersPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnLabOrdersPlusSign()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnLabOrdersPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);


                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnRadiologyOrdersPlusSign()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyOrdersPlusSign", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void CollapseAllExpandedOrderSections()
        {
            ClickOnPlanNotesPlusSign();
            ClickOnHomeActiveMedicationsPlusSign();
            ClickOnCPOEPlusSign();
            ClickOnOrderSetPlusSign();
            ClickOnMedicationOrdersPlusSign();
            ClickOnLabOrdersPlusSign();
            ClickOnRadiologyOrdersPlusSign();
        }
        #endregion CollapseExpand order section +,- icon

        #region Lab Orders Section
        private void CloseLabOrders()
        {
            var closeLabOrdersSectionData = _configReaderService.GetSectionData("CloseLabOrders", _config, true, false, false);

            if (closeLabOrdersSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeLabOrdersSectionData.InitialDelay;
                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)closeLabOrdersSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeLabOrdersSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnLabOrderShowMoreBtn()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnLabOrderShowMoreBtn", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                Thread.Sleep(500);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                Thread.Sleep(1000);
            }
        }
        //private void ClickOnLabOrderShowLessBtn()
        //{
        //    var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnLabOrderShowLessBtn", _config, true, false, false);

        //    if (closeDiagnosisTabSectionData != null)
        //    {
        //        int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

        //        // Set the coordinates where you want to click
        //        int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //        int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //        Thread.Sleep(initialThreadSleepDuration);

        //        // Move the mouse cursor to the specific location
        //        Cursor.Position = new System.Drawing.Point(x, y);

        //        // Perform left mouse button down and up actions
        //        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //    }
        //}
        //private void ClickOnLabReportAddMore()
        //{
        //    var clickOnAddMoreSectionData = _configReaderService.GetSectionData("ClickOnLabOrdersAddMore", _config, true, false, false);

        //    if (clickOnAddMoreSectionData != null)
        //    {
        //        int initialThreadSleepDuration = (int)clickOnAddMoreSectionData.InitialDelay;
        //        int afterClickDelay = (int)clickOnAddMoreSectionData.SectionConfig["AfterClickDelay"]["Halt"];
        //        Thread.Sleep(initialThreadSleepDuration);

        //        // Set the coordinates where you want to click
        //        int x = (int)clickOnAddMoreSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //        int y = (int)clickOnAddMoreSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //        // Move the mouse cursor to the specific location
        //        Cursor.Position = new System.Drawing.Point(x, y);

        //        // Perform left mouse button down and up actions
        //        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

        //        Thread.Sleep(afterClickDelay);
        //    }
        //}
        private void ClickOnLabOrderGridRow()
        {

            var clickOnLabOrderGridRowSectionData = _configReaderService.GetSectionData("ClickOnLabOrderGridRow", _config, true, true, false);

            if (clickOnLabOrderGridRowSectionData != null)
            {
                int initialThreadSleepDuration = (int)clickOnLabOrderGridRowSectionData.InitialDelay;
                int doubleClickDelay = (int)clickOnLabOrderGridRowSectionData.DoubleClickDelay;
                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)clickOnLabOrderGridRowSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnLabOrderGridRowSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

                Thread.Sleep(doubleClickDelay);

                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnLabOrderSearchBar()
        {
            var clickOnLabOrderSearchBarSectionData = _configReaderService.GetSectionData("ClickOnLabOrderSearchBar", _config, true, false, false);

            if (clickOnLabOrderSearchBarSectionData != null)
            {
                int initialThreadSleepDuration = (int)clickOnLabOrderSearchBarSectionData.InitialDelay;
                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)clickOnLabOrderSearchBarSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnLabOrderSearchBarSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void ClickOnLabOrders()
        {
            var clickOnLabOrdersSectionData = _configReaderService.GetSectionData("ClickOnLabOrders", _config, true, false, false);

            if (clickOnLabOrdersSectionData != null)
            {
                int initialThreadSleepDuration = (int)clickOnLabOrdersSectionData.InitialDelay;
                Thread.Sleep(initialThreadSleepDuration);

                // Set the coordinates where you want to click
                int x = (int)clickOnLabOrdersSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnLabOrdersSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        private void CheckIfAlreadyExistsPopupShows()
        {
            var clickOnLabOrdersSectionData = _configReaderService.GetSectionData("CheckIfAlreadyExistsPopupShows", _config, true, false, true);

            if (clickOnLabOrdersSectionData != null)
            {
                int initialThreadSleepDuration = (int)clickOnLabOrdersSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)clickOnLabOrdersSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnLabOrdersSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate
                int contextMenuActionsX = (int)clickOnLabOrdersSectionData.contextMenuActions.InitialX; // Replace with your desired Y coordinate
                int contextMenuActionsY = (int)clickOnLabOrdersSectionData.contextMenuActions.InitialY; // Replace with your desired Y coordinate
                int afterCopySleep = (int)clickOnLabOrdersSectionData.SectionConfig["AfterCopySleep"]["Halt"]; // Replace with your desired Y coordinate
                int dragDistance = (int)clickOnLabOrdersSectionData.SectionConfig["DragMouseRightToLeft"]["Distance"]; // Replace with your desired Y coordinate

                Clipboard.Clear();
                // Text to be stored in the clipboard
                string textToCopy = " ";

                // Call the SetText method to store the text in the clipboard
                Clipboard.SetText(textToCopy);

                Thread.Sleep(initialThreadSleepDuration);
            
                // Move the mouse cursor to the starting position
                Cursor.Position = new Point(x, y);

                // Simulate dragging from right to left and then moving upward
                DragMouseRightToLeft(dragDistance); // Adjust the distance as needed
                                           //MoveMouseUp(centerX, centerY - 5); // Adjust the upward distance as needed

                Console.WriteLine("Mouse drag and move completed.");

                SendKeys.SendWait("^c"); // "^" represents the Ctrl key
                SendKeys.SendWait("^c"); // "^" represents the Ctrl key

                Thread.Sleep(afterCopySleep);

                if (Clipboard.GetText().Contains("add again?"))
                {
                    Cursor.Position = new Point(contextMenuActionsX, contextMenuActionsY);
                    LeftMouseClick();
                }
            }
        }
        private void CheckIfHistoryRequiredPopupShows()
        {
            var clickOnLabOrdersSectionData = _configReaderService.GetSectionData("CheckIfHistoryRequiredPopupShows", _config, true, false, true);

            if (clickOnLabOrdersSectionData != null)
            {
                int initialThreadSleepDuration = (int)clickOnLabOrdersSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)clickOnLabOrdersSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)clickOnLabOrdersSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate
                int contextMenuActionsX = (int)clickOnLabOrdersSectionData.contextMenuActions.InitialX; // Replace with your desired Y coordinate
                int contextMenuActionsY = (int)clickOnLabOrdersSectionData.contextMenuActions.InitialY; // Replace with your desired Y coordinate
                int afterCopySleep = (int)clickOnLabOrdersSectionData.SectionConfig["AfterCopySleep"]["Halt"]; // Replace with your desired Y coordinate
                int dragDistance = (int)clickOnLabOrdersSectionData.SectionConfig["DragMouseRightToLeft"]["Distance"]; // Replace with your desired Y coordinate

                Clipboard.Clear();

                // Text to be stored in the clipboard
                string textToCopy = " ";

                // Call the SetText method to store the text in the clipboard
                Clipboard.SetText(textToCopy);

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the starting position
                Cursor.Position = new Point(x, y);

                // Simulate dragging from right to left and then moving upward
                DragMouseRightToLeft(dragDistance); // Adjust the distance as needed
                                           //MoveMouseUp(centerX, centerY - 5); // Adjust the upward distance as needed

                Console.WriteLine("Mouse drag and move completed.");

                SendKeys.SendWait("^c"); // "^" represents the Ctrl key
                SendKeys.SendWait("^c"); // "^" represents the Ctrl key
                Thread.Sleep(afterCopySleep);
                if (Clipboard.GetText() == "corroborating to the lab investigations requested")
                {
                    Cursor.Position = new Point(contextMenuActionsX, contextMenuActionsY);
                    LeftMouseClick();
                }
            }
        }
        #endregion Lab Orders Section

        #region Lab Order
        private void ClickOnDeleteLabOrders(int count)
        {
            string sectionKey = "ClickOnDeleteLabOrdersWithoutShowMore";
            if (count > 5)
            {
                sectionKey = "ClickOnDeleteLabOrders";
                ClickOnLabOrderShowMoreBtn();
            }

            ClickOnDeleteCommon(count, sectionKey);
        }
        internal void AddAndDeleteCPTInLabOrder(List<string> cptCodes, int removeRowCount, bool deletionRequired)
        {
            ClickOnLabOrders();

            if (deletionRequired)
            {
                ClickOnDeleteLabOrders(removeRowCount);
            }

            foreach (var code in cptCodes)
            {
                ClickOnLabOrderSearchBar();

                // Simulate Ctrl+A (Select All)
                SendKeys.SendWait("^a"); // "^" represents the Ctrl key
                SendKeys.SendWait("^a"); // "^" represents the Ctrl key

                // Simulate Backspace
                SendKeys.SendWait("{BACKSPACE}");
                SendKeys.SendWait("{BACKSPACE}");

                SearchCPT(code);
                ClickOnLabOrderGridRow();
                CheckIfHistoryRequiredPopupShows();
                CheckIfAlreadyExistsPopupShows();
                //ClickOnLabReportAddMore();
            }

            CloseLabOrders();
        }
        #endregion Lab Order

        #region api cosumption/calling generic methods goes here
        internal void HandleApiCall<T>(Func<ResponseModel<T>> apiCall, Action<ResponseModel<T>> successCallback)
        {
            var response = apiCall.Invoke();

            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Message == "SUCCESS")
            {
                successCallback.Invoke(response);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HandleUnauthorized(apiCall, successCallback);
            }
            else
            {
                HandleError(response);
            }
        }

        private void HandleUnauthorized<T>(Func<ResponseModel<T>> apiCall, Action<ResponseModel<T>> successCallback)
        {
            // Save auth token to app settings for later use
            AppSettingManager.Save("authToken", "");

            _authenticationService = new AuthenticationService(_apiBaseURl, _authUserId, _authLoginPassword);

            // RefreshAuthToken with the updated valueFactory
            RefreshAuthToken(() => _authenticationService.GetAuthToken());

            // Subscribe to the AuthTokenRetrieved event
            _authenticationService.AuthTokenRetrieved += OnAuthTokenRefreshed;

            try
            {
                // Retry the API call after refreshing the token
                var retryResponse = apiCall.Invoke();

                if (retryResponse.StatusCode == System.Net.HttpStatusCode.OK && retryResponse.Message == "SUCCESS")
                {
                    successCallback.Invoke(retryResponse);
                }
                else
                {
                    HandleError(retryResponse);
                }
            }
            finally
            {
                // Unsubscribe from the event when it's no longer needed
                _authenticationService.AuthTokenRetrieved -= OnAuthTokenRefreshed;
            }
        }

        private void HandleError<T>(ResponseModel<T> response)
        {
            _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
            _log.Error($"StatusCode: {response.StatusCode} Message: {response.Message} Token: {_authToken} while trying to consume api with url: {_apiBaseURl + "api/Octopus/PatientConditions"} and payload {response.Request}");
            MessageBox.Show("Something went wrong, Please contact system's administrator");
        }
        #endregion api cosumption/calling logic goes here

        #region AppSettings Read & Save
        // Later on, when you want to set _configFile again
        private void SetConfigFileName(string newConfigFile)
        {
            _configFile = newConfigFile;
        }

        private void SaveAppSettings(string key, string value)
        {
            // Update or add the key-value pair in the appSettingsDTO
            if (_appSettings != null)
            {
                // Assuming Configurations property is not null in AppSettingsDTO
                _appSettings.DeviceConfigurations = _appSettings.DeviceConfigurations ?? new Device();
                typeof(Device).GetProperty(key)?.SetValue(_appSettings.DeviceConfigurations, value);
            }

            // Save the updated appSettingsDTO to the JSON file
            SaveAppSettingsToFile();
        }

        private void SaveUpdatedResolution(string key, int width, int height)
        {
            // Update or add the key-value pair in the appSettingsDTO
            if (_appSettings != null)
            {
                // Assuming DeviceConfigurations property is not null in AppSettingsDTO
                _appSettings.DeviceConfigurations = _appSettings.DeviceConfigurations ?? new Device();
                typeof(Device).GetProperty(key)?.SetValue(_appSettings.DeviceConfigurations, $"{width}x{height}");
            }

            // Save the updated appSettingsDTO to the JSON file
            SaveAppSettingsToFile();
        }

        private void SaveAppSettingsToFile()
        {
            try
            {
                string currentDirectory = _currentWorkingDirectory;
                string fileName = _appSettingsFileName; // Your file name

                string filePath = Path.Combine(currentDirectory, fileName);

                // Serialize the appSettingsDTO to JSON
                string json = JsonConvert.SerializeObject(_appSettings, Formatting.Indented);

                // Write the JSON to the file
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving appsettings to file: " + e.Message);
            }
        }
        #endregion
        // Dispose method to unsubscribe from events if needed
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _authenticationService.AuthTokenRetrieved -= OnAuthTokenRetrieved;
        //    }

        //    base.Dispose(disposing);
        //}
    }
}