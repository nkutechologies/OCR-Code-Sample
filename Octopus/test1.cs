using Octopus.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus
{
    public partial class test1  : BaseForm
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
        public test1()
        {
            InitializeComponent();
        }

        private void test1_Load(object sender, EventArgs e)
        {
            ConvertResolution();
            //Sample strings
            //string string1 = "22110";
            string string11 = "74290";
            //string string2 = "71045";
            //string string3 = "73221";
            //string string31 = "74246";

            // Creating a List<string> and adding the strings
            List<string> stringList = new List<string>();
            //stringList.Add(string1);
            stringList.Add(string11);
            //stringList.Add(string2);
            //stringList.Add(string3);
            //stringList.Add(string31);
            AddAndDeleteCPTInRadiology(stringList,1,true);
        }
        //private void ClickOnDeleteRadiology(int cptCount)
        //{
        //    if (cptCount > 5)
        //    {
        //        ClickOnRadiologyShowMoreBtn();
        //        var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDeleteRadiology", _config, true, true, false);

        //        if (closeDiagnosisTabSectionData != null)
        //        {
        //            int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
        //            int doubleClickDelay = (int)closeDiagnosisTabSectionData.DoubleClickDelay;

        //            // Set the coordinates where you want to click
        //            int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //            int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //            Thread.Sleep(initialThreadSleepDuration);

        //            for (int i = 0; i < cptCount; i++)
        //            {
        //                Thread.Sleep(1000);
        //                // Move the mouse cursor to the specific location
        //                Cursor.Position = new System.Drawing.Point(x, y);

        //                // Perform left mouse button down and up actions
        //                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnDeleteRadiologyWithoutShowMore", _config, true, true, false);

        //        if (closeDiagnosisTabSectionData != null)
        //        {
        //            int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
        //            int doubleClickDelay = (int)closeDiagnosisTabSectionData.DoubleClickDelay;

        //            // Set the coordinates where you want to click
        //            int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
        //            int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

        //            Thread.Sleep(initialThreadSleepDuration);

        //            for (int i = 0; i < cptCount; i++)
        //            {
        //                Thread.Sleep(1000);
        //                // Move the mouse cursor to the specific location
        //                Cursor.Position = new System.Drawing.Point(x, y);

        //                // Perform left mouse button down and up actions
        //                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        //            }
        //        }
        //    }
        //}
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
        private void ClickOnRadiologyOrderAddMore()
        {

            var closeDiagnosisTabSectionData = _configReaderService.GetSectionData("ClickOnRadiologyOrderAddMore", _config, true, false, false);

            if (closeDiagnosisTabSectionData != null)
            {
                int initialThreadSleepDuration = (int)closeDiagnosisTabSectionData.InitialDelay;
                int afterClickDelay = (int)closeDiagnosisTabSectionData.SectionConfig["AfterClickDelay"]["Halt"]; // Replace with your desired Y coordinate

                // Set the coordinates where you want to click
                int x = (int)closeDiagnosisTabSectionData.Coordinates.InitialX; // Replace with your desired X coordinate
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);


                // Move the mouse cursor to the specific location
                Cursor.Position = new System.Drawing.Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

                Thread.Sleep(afterClickDelay);
            }
        }
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
                int y = (int)closeDiagnosisTabSectionData.Coordinates.InitialY ; // Replace with your desired Y coordinate

                Thread.Sleep(initialThreadSleepDuration);

                // Move the mouse cursor to the specific location
                Cursor.Position = new Point(x, y);

                // Perform left mouse button down and up actions
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
















        private void ClickOnDeleteRadiology(int cptCount)
        {
            var closeDiagnosisTabSectionDataKey = "ClickOnDeleteRadiologyWithoutShowMore";

            if (cptCount > 5)
            {
                closeDiagnosisTabSectionDataKey = "ClickOnDeleteRadiology";
                ClickOnRadiologyShowMoreBtn();
            }

            ClickOnDeleteRadiologyCommon(cptCount, closeDiagnosisTabSectionDataKey);
        }

        private void ClickOnDeleteRadiologyCommon(int cptCount, string sectionKey)
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

                for (int i = 0; i < cptCount; i++)
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
