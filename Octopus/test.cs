using Octopus.Common;
using SikuliModule;
using SikuliSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;

namespace Octopus
{
    public partial class test : BaseForm
    {
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
        public test()
        {
            InitializeComponent();
        }
        private void ClickOnDignosisSecondaryRadioBtn()
        {
            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDignosisSecondaryRadioBtn", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX+30; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the starting position
                Cursor.Position = new System.Drawing.Point(x, y);

                //click on Secondary radio button on dignosis tab
                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                //click on Secondary radio button on dignosis tab
            }
        }
        private void test_Load(object sender, EventArgs e)
        {
            ////There are 3 patterns on the test image
            //var points = SikuliAction.FindAll(@"D:\myscript.sikuli\1706431479249.png");
            //if (points != null)
            //{
            //    foreach (System.Drawing.Point point in points)
            //    {
            //        Console.WriteLine("X:" + point.X + "  Y: " + point.Y);
            //    }
            //    if (points.Count == 3)
            //    {
            //        Console.WriteLine("Yep! They are 3...");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Nope! They are NOT 3, they are " + points.Count);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Nope! There is a problem...");
            //}
            try
            {

                //check if current dignosis is collapsed or expanded
                using (var session = Sikuli.CreateSession())
                {
                    var patternPath = @"D:\myscripts1.sikuli\1706509189678.png";
                    var similarityThreshold = 0.9999f; // Adjust the similarity threshold as needed
                    var pattern = Patterns.FromFile(patternPath, similarityThreshold);
                    if (session.Exists(pattern))
                    {
                        // Define the target offset for the click action
                        var offset = new Point(-40, -2);

                        // Add your logic for actions to be taken when the image is found
                        session.Click(pattern, offset);
                    }
                    else
                    {
                        // Add your logic for actions to be taken when the image is not found
                    }
                }
                //click on observe 
                using (var session = Sikuli.CreateSession())
                {
                    var patternPath = @"D:\myscript.sikuli\1706431479249.png";
                    var similarityThreshold = 0.9999f; // Adjust the similarity threshold as needed

                    var pattern = Patterns.FromFile(patternPath, similarityThreshold);

                    if (session.Exists(pattern/*, similarityThreshold*/))
                    {
                        var submit = Patterns.FromFile(patternPath, 0.9f);

                        // Add your logic for actions to be taken when the image is found
                        session.Click(submit);
                    }
                    else
                    {
                        // Add your logic for actions to be taken when the image is not found
                    }
                }

            }
            catch (Exception x)
            {

            }
            //try
            //{
            //    if (SikuliAction.Exists(@"D:\source\repos\LifeTrenz octopus Git\Octopus-Aster\Octopus\Resources\FormAssets\Capture.PNG").IsEmpty)
            //    {
            //        Console.WriteLine("Nope! It's gone...");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Yep! It's there...");
            //    }
            //    SikuliAction.Click(@"D:\source\repos\LifeTrenz octopus Git\Octopus-Aster\Octopus\Resources\FormAssets\Capture.PNG");
            //}
            //catch (Exception x)
            //{

            //} 
                ConvertResolution();

            //Sample strings
            string string1 = "82043";
            //string string11 = "80061";
            //string string2 = "86038";
            //string string3 = "80051";
            //string string31 = "86140";
            //string string32 = "81000";
            //string string33 = "82565";
            //string string34 = "82950";
            //string string345 = "81001";
            //string string35 = "80069";
            //string string37 = "80076";

            //Creating a List<string> and adding the strings
            List<string> stringList = new List<string>();
            stringList.Add(string1);
            //stringList.Add(string11);
            //stringList.Add(string2);
            //stringList.Add(string3);
            //stringList.Add(string31);
            //stringList.Add(string32);
            //stringList.Add(string33);
            //stringList.Add(string34);
            //stringList.Add(string35);
            //stringList.Add(string345);
            //stringList.Add(string37);

            AddAndDeleteCPTInLabOrder(stringList,1, true);
            //CheckIfAlreadyExistsPopupShows();

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

                System.Windows.Forms.Clipboard.Clear();
                // Text to be stored in the clipboard
                string textToCopy = " ";

                // Call the SetText method to store the text in the clipboard
                System.Windows.Forms.Clipboard.SetText(textToCopy);

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the starting position
                Cursor.Position = new System.Drawing.Point(x, y);

                // Simulate dragging from right to left and then moving upward
                DragMouseRightToLeft(0, 0, dragDistance); // Adjust the distance as needed
                                                          //MoveMouseUp(centerX, centerY - 5); // Adjust the upward distance as needed

                Console.WriteLine("Mouse drag and move completed.");

                SendKeys.SendWait("^c"); // "^" represents the Ctrl key
                SendKeys.SendWait("^c"); // "^" represents the Ctrl key

                Thread.Sleep(afterCopySleep);

                if (System.Windows.Forms.Clipboard.GetText().Contains("add again?"))
                {
                    Cursor.Position = new System.Drawing.Point(contextMenuActionsX, contextMenuActionsY);
                    LeftMouseClick(0, 0);
                }
            }
        }
        private void CheckIfHistoryRequiredPopupShows()
        {
            System.Windows.Forms.Clipboard.Clear();

            // Text to be stored in the clipboard
            string textToCopy = " ";

            // Call the SetText method to store the text in the clipboard
            System.Windows.Forms.Clipboard.SetText(textToCopy);

            int initialThreadSleepDuration = 1000;
            Thread.Sleep(initialThreadSleepDuration);

            // Get screen dimensions
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Calculate the center coordinates
            int centerX = screenWidth / 2 + 257;
            int centerY = screenHeight / 2 - 10;

            // Move the mouse cursor to the starting position
            Cursor.Position = new System.Drawing.Point(centerX, centerY);

            // Simulate dragging from right to left and then moving upward
            DragMouseRightToLeft(centerX, centerY, 330); // Adjust the distance as needed
            //MoveMouseUp(centerX, centerY - 5); // Adjust the upward distance as needed

            Console.WriteLine("Mouse drag and move completed.");

            SendKeys.SendWait("^c"); // "^" represents the Ctrl key
            SendKeys.SendWait("^c"); // "^" represents the Ctrl key
            Thread.Sleep(2000);
            if (System.Windows.Forms.Clipboard.GetText().Contains("corroborating to the lab investigations requested"))
            {
                Cursor.Position = new System.Drawing.Point(centerX - 220, centerY + 100);
                LeftMouseClick(centerX - 220, centerY + 100);
            }
        }
        private void AddAndDeleteCPTInLabOrder(List<string> cptCodes, int removeRowCount, bool deletionRequired)
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
        //private void ClickOnDeleteLabOrders(int count)
        //{
        //    if (count > 5)
        //    {
        //        ClickOnLabOrderShowMoreBtn();
        //        for (int i = 0; i < count; i++)
        //        {
        //            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDeleteLabOrders", _config, true, false, true);

        //            if (closeDiagnosisTabSectionData != null)
        //            {
        //                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

        //                // Set the coordinates where you want to click
        //                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //                Thread.Sleep(initialThreadSleepDuration);

        //                // Move the mouse cursor to the specific location
        //                Cursor.Position = new System.Drawing.Point(x, y);

        //                // Perform left mouse button down and up actions
        //                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDeleteLabOrdersWithoutShowMore", _config, true, false, true);

        //        if (closeDiagnosisTabSectionData != null)
        //        {
        //            int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;

        //            // Set the coordinates where you want to click
        //            int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //            int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //            Thread.Sleep(initialThreadSleepDuration);

        //            // Move the mouse cursor to the specific location
        //            Cursor.Position = new System.Drawing.Point(x, y);

        //            // Perform left mouse button down and up actions
        //            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //        }
        //    }
        //    //ClickOnLabOrderShowLessBtn();
        //}
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
        static void DragMouseRightToLeft(int startX, int startY, int distance)
        {
            InputSimulator inputSimulator = new InputSimulator();
            inputSimulator.Mouse.LeftButtonDown();
            inputSimulator.Mouse.MoveMouseBy(distance * -1, 0);
            inputSimulator.Mouse.LeftButtonUp();

            // Optional: Sleep to add a delay between actions
            Thread.Sleep(500);
        }
        static void MoveMouseUp(int x, int y)
        {
            InputSimulator inputSimulator = new InputSimulator();
            inputSimulator.Mouse.MoveMouseTo(x, y);

            // Optional: Sleep to add a delay between actions
            Thread.Sleep(500);
        }
        static void LeftMouseClick(int x, int y)
        {
            InputSimulator inputSimulator = new InputSimulator();
            inputSimulator.Mouse.LeftButtonClick();
        }




        static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);

            // Optional: Sleep to add a delay between actions
            Thread.Sleep(500);
        }

        static void MouseClickLeft(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);

            // Optional: Sleep to add a delay between actions
            Thread.Sleep(500);
        }

        static void DragMouseWithLoop(int startX, int startY, int distance, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int newX = startX - i * distance; // Adjust as needed
                int newY = startY;

                SetCursorPosition(newX, newY);

                // Optional: Sleep to add a delay between iterations
                Thread.Sleep(200);
            }
        }

        static void MoveMouseUp1(int x, int y)
        {
            SetCursorPosition(x, y);

            // Optional: Sleep to add a delay between actions
            Thread.Sleep(500);
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);


        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);










        private void ClickOnDeleteLabOrders(int count)
        {
            string sectionKey = "ClickOnDeleteLabOrdersWithoutShowMore";
            if (count > 5)
            {
                sectionKey = "ClickOnDeleteLabOrders";
                ClickOnLabOrderShowMoreBtn();
            }

            ClickOnDeleteLabOrdersCommon(count, sectionKey);
        }

        private void ClickOnDeleteLabOrdersCommon(int count, string sectionKey)
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
                    Cursor.Position = new System.Drawing.Point(x, y);

                    // Perform left mouse button down and up actions
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                    Thread.Sleep(500);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

                    Thread.Sleep(doubleClickDelay);
                }
            }
        }
    }
}
