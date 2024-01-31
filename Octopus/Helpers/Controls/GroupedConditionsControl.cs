using Octopus.Dtos.ConditionsDtos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;
using CheckBox = System.Windows.Forms.CheckBox;
using Label = System.Windows.Forms.Label;

namespace Octopus.Helpers.Controls
{

    public class GroupedConditionsControl : UserControl
    {
        private string groupId;
        private List<string> conditions;

        public GroupedConditionsControl(string groupId, List<string> conditions)
        {
            this.groupId = groupId;
            this.conditions = conditions;
            InitializeControls();
        }

        private void InitializeControls()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.AutoSize = true;

            int maxConditionsToShow = 2; // Maximum number of conditions to display initially

            for (int i = 0; i < conditions.Count; i += maxConditionsToShow)
            {
                FlowLayoutPanel rowPanel = new FlowLayoutPanel();
                rowPanel.FlowDirection = FlowDirection.LeftToRight;
                rowPanel.AutoSize = true;

                for (int j = i; j < i + maxConditionsToShow && j < conditions.Count; j++)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.AutoSize = true;

                    Label label = new Label();
                    label.Text = conditions[j];
                    label.AutoSize = true;

                    rowPanel.Controls.Add(checkBox);
                    rowPanel.Controls.Add(label);
                }

                flowLayoutPanel.Controls.Add(rowPanel);
            }

            if (conditions.Count > maxConditionsToShow)
            {
                Button showMoreButton = new Button();
                showMoreButton.Text = "Show More";
                showMoreButton.Click += ShowMore_Click;

                flowLayoutPanel.Controls.Add(showMoreButton);
            }

            this.Controls.Add(flowLayoutPanel);
        }

        private void ShowMore_Click(object sender, EventArgs e)
        {
            // Logic to handle "Show More" click
            // Fetch global data using groupId and update UI to display additional conditions
        }
    }

}
