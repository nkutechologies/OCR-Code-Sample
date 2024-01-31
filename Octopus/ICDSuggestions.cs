using Newtonsoft.Json;
using Octopus.Common;
using Octopus.Dtos.Common;
using Octopus.Dtos.ConditionsDtos;
using Octopus.Dtos.ICDCodeDtos;
using Octopus.Dtos.ICDSuggestionsDtos;
using Octopus.Dtos.PatientConditionsDtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Octopus
{
    public partial class ICDSuggestions : BaseForm, IDisposable
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ICDSuggestions));
        private PatientConditionsDto _patientConditionsDto = new PatientConditionsDto();
        private readonly List<GroupedConditionsDto> _groupedConditions;
        // Declare tableLayoutPanel1 at the class level
        TableLayoutPanel tableLayoutPanel1 = new TableLayoutPanel();
        private int selectedCheckboxCount = 0;
        public event Action IcdOperationFinished;
        public ICDSuggestions(List<GroupedConditionsDto> groupedConditions, PatientConditionsDto patientConditionsDto)
        {
            try
            {
                _patientConditionsDto = patientConditionsDto;
                InitializeComponent();
                pictureBox1.MouseDown += pictureBox1_MouseDown;
                pictureBox1.MouseMove += pictureBox1_MouseMove;
                pictureBox1.MouseUp += pictureBox1_MouseUp;

                // Assuming you have a reference to the form you want to bring to the front

                // Disable resizing and maximize button
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;

                // Set the form's start position to center screen
                this.StartPosition = FormStartPosition.CenterScreen;

                this.TopMost = true; // Ensure it's displayed above other windows
                this.Activate(); // Ensure the dialog gains focus
                this.BringToFront();
                SetForegroundWindow(this.Handle);

                _groupedConditions = groupedConditions;

                // Initialize selectedCheckboxCount based on the initially checked checkboxes
                selectedCheckboxCount = groupCheckboxes.Values.Sum(group => group.Count(checkbox => checkbox.Checked));

                label6.Text = $"({selectedCheckboxCount}) out of {_groupedConditions.Sum(x => x.Conditions.Count)} is been selected";

                SetupTableLayoutPanel();
            }
            catch (Exception ex)
            {
                _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
                _log.Error("Error Message: " + ex.Message.ToString(), ex);
            }
        }

        Dictionary<string, List<CheckBox>> groupCheckboxes = new Dictionary<string, List<CheckBox>>();

        #region Initialization code for Suggestion popup Panel
        private void SetupTableLayoutPanel()
        {
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            //tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f)); // Divide into three columns
            //tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            //tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            tableLayoutPanel1.Padding = new Padding(10); // Adjust the padding as needed

            // Adding an empty row at the beginning to create space
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 6)); // Adjust height for desired space

            // Increment row count for the empty row
            tableLayoutPanel1.RowCount++;

            Controls.Add(tableLayoutPanel1);

            // Create a dictionary to store checkbox-label associations
            Dictionary<Tuple<string, int>, CheckBox> checkboxDict = new Dictionary<Tuple<string, int>, CheckBox>();

            for (int j = 0; j < _groupedConditions.Count; j++)
            {
                int capturedIndex = j;
                var currentGroup = _groupedConditions[j];

                var flag = false;
                int conditionCount = 0;
                int rowIndex = tableLayoutPanel1.RowCount;

                tableLayoutPanel1.RowCount += 2; // Increase row count for the group label and checkbox rows

                for (int i = 0; i < _groupedConditions[j].Conditions.Count; i++)
                {
                    if (conditionCount < 3) // Show only the first three conditions initially
                    {
                        if (conditionCount % 3 == 0)
                        {
                            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            tableLayoutPanel1.RowCount++;
                        }

                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = _groupedConditions[j].Conditions[i].Condition;
                        checkBox.AutoSize = true;
                        checkBox.Margin = new Padding(10); // Adjust the margin as needed
                        checkBox.Padding = new Padding(5); // Padding inside the checkbox control
                        checkBox.TextAlign = ContentAlignment.MiddleLeft; // Center-align the text

                        // Attach CheckedChanged event handler
                        checkBox.CheckedChanged += (sender, e) => CheckBox_CheckedChanged(sender, e, _groupedConditions[capturedIndex].GroupId);

                        // Store checkbox-label associations in the dictionary
                        checkboxDict[new Tuple<string, int>(_groupedConditions[j].GroupId, i)] = checkBox;

                        // Store checkboxes in the groupCheckboxes dictionary
                        if (!groupCheckboxes.ContainsKey(_groupedConditions[j].GroupId))
                        {
                            groupCheckboxes[_groupedConditions[j].GroupId] = new List<CheckBox>();
                        }
                        groupCheckboxes[_groupedConditions[j].GroupId].Add(checkBox);

                        tableLayoutPanel1.Controls.Add(checkBox, conditionCount % 3, rowIndex);

                        if (_groupedConditions[j].Conditions.Count > 3)
                        {
                            if (!flag)
                            {
                                LinkLabel showMoreButton = new LinkLabel();
                                showMoreButton.Text = "Show More";
                                showMoreButton.Tag = _groupedConditions[j].GroupId;
                                showMoreButton.Click += (sender, e) => ShowMoreButton_Click(sender, e, tableLayoutPanel1, checkboxDict);
                                showMoreButton.Margin = new Padding(5);
                                tableLayoutPanel1.Controls.Add(showMoreButton, conditionCount % 3 + 2, rowIndex);
                            }
                            flag = true;
                        }

                        conditionCount++;

                        if (conditionCount >= 3)
                        {
                            rowIndex++;
                        }
                    }
                    else // Hide additional checkboxes initially
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = _groupedConditions[j].Conditions[i].Condition;
                        checkBox.AutoSize = true;
                        checkBox.Margin = new Padding(10); // Adjust the margin as needed
                        checkBox.Padding = new Padding(5); // Padding inside the checkbox control
                        checkBox.Visible = false;
                        checkBox.TextAlign = ContentAlignment.MiddleLeft; // Center-align the text

                        // Store checkbox-label associations in the dictionary
                        checkboxDict[new Tuple<string, int>(_groupedConditions[j].GroupId, i)] = checkBox;

                        // Store checkboxes in the groupCheckboxes dictionary
                        if (!groupCheckboxes.ContainsKey(_groupedConditions[j].GroupId))
                        {
                            groupCheckboxes[_groupedConditions[j].GroupId] = new List<CheckBox>();
                        }
                        groupCheckboxes[_groupedConditions[j].GroupId].Add(checkBox);

                        tableLayoutPanel1.Controls.Add(checkBox, conditionCount % 3, rowIndex);
                    }
                }

                // Add a separator line only if it's a new group
                if (j < _groupedConditions.Count - 1 && !string.Equals(currentGroup.GroupId, _groupedConditions[j + 1].GroupId, StringComparison.OrdinalIgnoreCase))
                {
                    var separatorLine = new Label
                    {
                        AutoSize = false,
                        Height = 2, // Adjust height for the space between groups
                        Width = tableLayoutPanel1.ClientSize.Width,
                        BackColor = Color.Black,
                        BorderStyle = BorderStyle.Fixed3D,
                        // Additional padding for space above and below the separator line
                        Padding = new Padding(10, 20, 10, 20),
                    };

                    int rowIndexForSeparator = tableLayoutPanel1.RowCount++;
                    tableLayoutPanel1.Controls.Add(separatorLine, 0, rowIndexForSeparator);
                    tableLayoutPanel1.SetColumnSpan(separatorLine, 3); // Span the line across all columns
                }
            }

            panel1.Controls.Add(tableLayoutPanel1);
        }

        private void ShowMoreButton_Click(object sender, EventArgs e, TableLayoutPanel tableLayoutPanel, Dictionary<Tuple<string, int>, CheckBox> checkboxDict)
        {
            var button = sender as LinkLabel;
            button.Visible = false;

            if (button != null && button.Tag != null)
            {
                string groupId = button.Tag.ToString();

                int buttonRowIndex = tableLayoutPanel.GetRow(button);

                int conditionCount = 0; // Counter for the checkboxes added below the button
                int remainingCells = 3; // Remaining cells in the current row

                // Shift the existing checkboxes in the clicked group to the next row
                foreach (var kvp in checkboxDict)
                {
                    Tuple<string, int> key = kvp.Key;
                    CheckBox checkBox = kvp.Value;

                    if (key.Item1 == groupId && !checkBox.Visible)
                    {
                        // Attach CheckedChanged event handler to the initially hidden checkboxes
                        checkBox.CheckedChanged += (s, ev) => CheckBox_CheckedChanged(s, ev, groupId);

                        int row = tableLayoutPanel.GetRow(checkBox);

                        // Check if the checkbox is below or in the same row as the button
                        if (row >= buttonRowIndex)
                        {
                            // Place the checkboxes below the button in the next cell of the current row
                            tableLayoutPanel.SetRow(checkBox, buttonRowIndex);
                            tableLayoutPanel.SetColumn(checkBox, 3 - remainingCells);

                            checkBox.Visible = true; // Show the checkbox in the new cell
                            conditionCount++;
                            remainingCells--;

                            // Check if the current row is full, move to the next row
                            if (remainingCells == 0)
                            {
                                tableLayoutPanel.RowCount++;
                                buttonRowIndex++;
                                remainingCells = 3;

                                // Check if the checkbox is in the same row as the button
                                if (row == buttonRowIndex)
                                {
                                    // If the checkbox is in the same row, move it to the next row
                                    tableLayoutPanel.SetRow(checkBox, buttonRowIndex);
                                    tableLayoutPanel.SetColumn(checkBox, 0);
                                    remainingCells = 3; // Adjust remaining cells accordingly
                                }
                            }
                        }
                    }
                }

                // Check if there are remaining checkboxes from the first group
                foreach (var kvp in checkboxDict)
                {
                    Tuple<string, int> key = kvp.Key;
                    CheckBox checkBox = kvp.Value;

                    if (key.Item1 == groupId && !checkBox.Visible)
                    {
                        // Attach CheckedChanged event handler to the initially hidden checkboxes
                        checkBox.CheckedChanged += (s, ev) => CheckBox_CheckedChanged(s, ev, groupId);

                        // Place the remaining checkboxes in the next row
                        tableLayoutPanel.SetRow(checkBox, buttonRowIndex);
                        tableLayoutPanel.SetColumn(checkBox, 3 - remainingCells);
                        checkBox.Visible = true;
                        conditionCount++;
                        remainingCells--;

                        // Check if the current row is full, move to the next row
                        if (remainingCells == 0)
                        {
                            tableLayoutPanel.RowCount++;
                            buttonRowIndex++;
                            remainingCells = 3;
                        }
                    }
                }
            }
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e, string groupId)
        {
            var clickedCheckbox = sender as CheckBox;

            // Check if the clicked checkbox is now checked
            if (clickedCheckbox.Checked)
            {
                // Uncheck other checkboxes in the same group
                foreach (var checkbox in groupCheckboxes[groupId])
                {
                    if (checkbox != clickedCheckbox)
                    {
                        checkbox.Checked = false;
                    }
                }
            }
            // Update the selectedCheckboxCount
            selectedCheckboxCount = groupCheckboxes.Values.Sum(group => group.Count(checkbox => checkbox.Checked));

            // Update the label text
            label6.Text = $"({selectedCheckboxCount}) out of {_groupedConditions.Sum(x => x.Conditions.Count)} is been selected";
        }

        #endregion Initialization code for Suggestion popup Panel

        private void ICDSuggestions_Load(object sender, EventArgs e)
        {
           
            // Remove the title bar and border
            FormBorderStyle = FormBorderStyle.None;
            // Hide the minimize, maximize, and close buttons
            ControlBox = false;

            // Set the ForeColor property of the label
            label1.Font = new Font("Montserrat", 8, FontStyle.Regular); // Assuming "Montserrat" is available in your system fonts
            label1.ForeColor = ColorTranslator.FromHtml("#392b65");
            label1.Font = new Font(label1.Font, FontStyle.Bold); // Set font weight to bold (600)

         
            label3.Font = new Font("Montserrat", 8, FontStyle.Regular);
            // Set the label's ForeColor property to the created color
         
            //label2.Font = new Font("Montserrat", 7, FontStyle.Regular); // Assuming "Montserrat" is available in your system fonts
            //label2.ForeColor = ColorTranslator.FromHtml("#392b65");
            //label2.Font = new Font(label2.Font, FontStyle.Bold); // Set font weight to bold (500)

    
            label3.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));
            label1.TabStop = true;
            //label2.TabStop = true;
            label3.TabStop = true;
            label3.Text = $"Copyright © {DateTime.Now.Year} Powered By NTIGRA.";

            //label4.Text = "------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
            //label4.Font = new System.Drawing.Font(Font.FontFamily, 1, FontStyle.Regular);
            //label4.Size = new System.Drawing.Size(1, 1);

            //label4.AutoSize = true;
            //label4.Width = tableLayoutPanel1.ClientSize.Width;
            //label4.Height = 1;


            label5.Text = "";
            label5.AutoSize = false;
            label5.Size = new Size(tableLayoutPanel1.ClientSize.Width+9, 1); // Set the width to match the container's width
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

            label6.Font = new Font("Montserrat", 8, FontStyle.Regular);
            //label6.ForeColor = ColorTranslator.FromHtml("#392b65");
            //label6.ForeColor = Color.Black;
            label6.ForeColor = ControlPaint.DarkDark(ColorTranslator.FromHtml("#392b65"));
            label6.BackColor = Color.Transparent;
            label6.Paint += (senderr, ee) => { };
            label6.TabStop = true;

            var sizeOfScreen = CurrentDisplayResolution;

            // Check the screen resolution and apply adjustments accordingly
            if (sizeOfScreen.Width <= 1400 && sizeOfScreen.Height <= 1050)
            {
                this.Size = new Size(923, 499);
            }
            // No need for an else block if there's no action for resolutions greater than 1400x1050

            //// Check the screen resolution and apply adjustments accordingly
            //if ((sizeOfScreen.Width >= 1920 && sizeOfScreen.Height >= 1080) ||
            //    (sizeOfScreen.Width >= 1680 && sizeOfScreen.Height >= 1050) ||
            //    (sizeOfScreen.Width >= 1600 && sizeOfScreen.Height >= 900) ||
            //    (sizeOfScreen.Width >= 1440 && sizeOfScreen.Height >= 900))
            //{
            //}
            //else
            //{
            //    this.Size = new Size(923, 499);
            //}
        }
        //private List<string> GetSelectedCheckBoxesText()
        //{
        //    return panel1.Controls.OfType<CheckBox>()
        //                          .Where(checkBox => checkBox.Checked)
        //                          .Select(checkBox => checkBox.Text)
        //                          .ToList();
        //}
        // Retrieve selected checkboxes' text from tableLayoutPanel1
        private List<string> GetSelectedCheckBoxesText()
        {
            return tableLayoutPanel1.Controls
                                .OfType<CheckBox>()
                                .Where(checkBox => checkBox.Checked)
                                .Select(checkBox => checkBox.Text)
                                .ToList();
        }

        #region accept button click handler code goes here
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //accept button click handler code goes here

            //fetch selectedCheckboxes text from suggestion winform dialogue so that we can pass it in the payload of octupus api in order to fetch icdcodes using endpoint "/api/Octopus/GetICDSuggestion"
            List<string> selectedConditions = GetSelectedCheckBoxesText();

            //hide this form
            this.Hide();

            MouseScrollUp();

            if (selectedConditions.Count() == 0)
            {
                Dispose();
                //this.Close();
                IcdOperationFinished.Invoke();
                return;
            }

            //pass those selected conditions to api using the generic method handleApiCall
            HandleApiCall(() => Services.ICDSuggestionsServices.ICDSuggestionsService.Get(
                JsonConvert.SerializeObject(new ICDSuggestionDto
                {
                    Conditions = selectedConditions,
                    DOB = _patientConditionsDto.DOB,
                    Gender = _patientConditionsDto.Gender,
                    Text = _patientConditionsDto.Text,
                    ChiefComplaint = _patientConditionsDto.Text
                }), _apiBaseURl, AuthToken), AddIcdsInDignosis);
        }
         #endregion accept button click handler code goes here
        private void AddIcdsInDignosis(ResponseModel<List<ICDCodesDto>> response)
        {
            AddAndDeleteICDinDignosisTab(response.Data.Select(x => x.Code).ToList(), removeRowCount: 0, deletionRequired: false, primaryAdditionRequired: false);
            IcdOperationFinished.Invoke();
        }

        #region Reject button click handler code goes here
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            IcdOperationFinished.Invoke();
            //reject button click handler code goes here
            this.Hide();
            Dispose(true);
            //this.Close();
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ICDSuggestions()
        {
            Dispose(false);
        }

        private void ICDSuggestions_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void ICDSuggestions_Resize(object sender, EventArgs e)
        {
          //label4.Location = new Point(label4.Location.X-30,label4.Location.Y);
            //label4.Width = panel1.Width;
        }
    }
}
