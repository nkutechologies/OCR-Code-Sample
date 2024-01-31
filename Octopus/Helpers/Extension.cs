using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Octopus.Helpers.CustomHelpers
{
    public class Extension : IExtension
    {
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Extension));

        public void ShowNotification(string notifyText)
        {
            try
            {
                var notification = new NotifyIcon()
                {
                    Visible = true,
                    Icon = SystemIcons.Information,
                     //optional -
                     BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                    // optional - 
                     BalloonTipTitle = "Octopus",
                };
                if (notification != null)
                {
                    notification.BalloonTipText = notifyText;
                    notification.ShowBalloonTip(500);
                }
                notification.Dispose();
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }
        }
        public Image LoadImage(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    return Image.FromFile(imagePath);
                }
                else
                {
                    _log.Info("Image you are trying to find at path= " + imagePath + " cannot be found");
                    // Handle the case when the image file doesn't exist.
                    return null; // Or any default image you want to use
                }
            }
            catch (FileNotFoundException ex)
            {
                _log.Error("Image file not found: " + ex.Message, ex);
            }
            catch (OutOfMemoryException ex)
            {
                _log.Error("Out of memory error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _log.Error("Error loading image: " + ex.Message, ex);
            }
            return null;
        }
        public string GetCurrentWorkingDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            return currentDirectory;
        }
        public Image LoadHighQualityImage(string imagePath, int width, int height)
        {
            using (Image img = Image.FromFile(imagePath))
            {
                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(img, 0, 0, width, height);
                }
                return bitmap;
            }
        }
        public Region GetRoundedRegion(Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rectangle.X, rectangle.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(rectangle.Right - cornerRadius * 2, rectangle.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(rectangle.Right - cornerRadius * 2, rectangle.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddArc(rectangle.X, rectangle.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }
        public void CustomizePanel(Panel panel)
        {
            // Set the BackColor property to white
            panel.BackColor = Color.White;

            // Set the Region property to create rounded corners
            int radius = 20; // Adjust the radius as needed
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, panel.Height - radius, radius, radius, 90, 90);
            panel.Region = new Region(path);
        }
        public void CustomizeLabel(Label label)
        {
            // Increase the font size and set it to bold
            label.Font = new Font(label.Font, FontStyle.Bold | FontStyle.Italic); // You can also use FontStyle.Bold if you want only bold

            // Optionally, you can increase the font size
            label.Font = new Font(label.Font.FontFamily, label.Font.Size + 8); // Adjust the size as needed
        }
    }
}
