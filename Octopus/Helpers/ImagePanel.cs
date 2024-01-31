using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus.Helpers.CustomHelpers
{
    public class ImagePanel : Panel
    {
        private Image image;

        public Image Image
        {
            get { return image; }
            set
            {
                image = value;
                // Call Invalidate to trigger the control to redraw with the new image
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (image != null)
            {
                // Draw the image on the panel
                e.Graphics.DrawImage(image, ClientRectangle);
            }
        }
    }

}
