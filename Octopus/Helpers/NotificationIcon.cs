using Octopus.Helpers.CustomHelpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Octopus.Helpers
{
   public class NotificationIcon
    {
        private NotifyIcon _notifyIcon;
        private IExtension _extensionService;

        public NotificationIcon(string imagePath, string tipIcon, string tipText, string tipTitle)
        {
            _extensionService = new Extension();
            _notifyIcon = new NotifyIcon();
            // Convert Image to Icon
            Icon iconFromImage = Icon.FromHandle((_extensionService.LoadImage(imagePath) as Bitmap).GetHicon());
            _notifyIcon.Icon = iconFromImage;
            _notifyIcon.Text = tipText;
            _notifyIcon.BalloonTipIcon = (ToolTipIcon)Enum.Parse(typeof(ToolTipIcon), tipIcon);
            _notifyIcon.BalloonTipTitle = tipTitle;
            _notifyIcon.Visible = false;

            // Hook up event handlers here if needed
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        }
        // Method to handle mouse click on the notification icon
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            // Handle mouse click behavior here
        }

        // Method to show the notification icon
        public void Show()
        {
            _notifyIcon.Visible = true;
        }

        // Method to hide the notification icon
        public void Hide()
        {
            _notifyIcon.Visible = false;
        }

        // Method to dispose the notification icon
        public void Dispose()
        {
            _notifyIcon.Dispose();
        }
    }
}
