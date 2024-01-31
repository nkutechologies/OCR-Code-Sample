using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus.Helpers.Controls
{
    public class GradientLabel : Label
    {
        public GradientLabel()
        {
            // Set up the label properties
            this.Font = new Font("Montserrat", 14, FontStyle.Regular); // Assuming "Montserrat" is available in your system fonts
            this.ForeColor = Color.Azure;

            // Attach the Paint event handler
            this.Paint += GradientLabel_Paint;
        }

        private void GradientLabel_Paint(object sender, PaintEventArgs e)
        {
            // Draw a gradient background
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(115, 57, 142), Color.FromArgb(57, 43, 101), LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            // Draw the label text
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
    }
}
