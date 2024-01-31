using System;
using System.Drawing;
using System.Windows.Forms;

namespace Octopus.Helpers.Controls
{
    public partial class MultilineCheckBox : UserControl
    {
        private CheckBox checkBox;
        private Label label;

        public MultilineCheckBox()
        {
            checkBox = new CheckBox();
            label = new Label();

            label.AutoSize = true;
            label.Dock = DockStyle.Fill;
            label.Padding = new Padding(5);
            label.TextAlign = ContentAlignment.MiddleLeft;

            checkBox.Dock = DockStyle.Left;

            this.Controls.Add(checkBox);
            this.Controls.Add(label);
        }

        public string Text
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public bool Checked
        {
            get { return checkBox.Checked; }
            set { checkBox.Checked = value; }
        }

        public event EventHandler CheckedChanged
        {
            add { checkBox.CheckedChanged += value; }
            remove { checkBox.CheckedChanged -= value; }
        }
    }

}
