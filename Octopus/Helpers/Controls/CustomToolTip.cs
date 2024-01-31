using System.Drawing;
using System.Windows.Forms;

namespace Octopus.Helpers.Controls
{
    public class CustomToolTip : Form
    {
        private Label label;

        public CustomToolTip(string text)
        {
            label = new Label();
            label.Text = text;
            label.ForeColor = Color.White; // Change Color.Red to your desired color
            label.AutoSize = true;

            // You can customize other properties of the form here, such as the background color, etc.
            BackColor = Color.Black;

            Controls.Add(label);
        }
    }
}
