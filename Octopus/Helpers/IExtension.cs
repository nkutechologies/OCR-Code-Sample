using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus.Helpers.CustomHelpers
{
    public interface IExtension
    {
        void CustomizePanel(Panel panel);
        void ShowNotification(string NotifiyText);
        Image LoadImage(string imagePath);
        string GetCurrentWorkingDirectory(); 
        Image LoadHighQualityImage(string imagePath, int width, int height);
        Region GetRoundedRegion(Rectangle rectangle, int cornerRadius);
        void CustomizeLabel(Label label);
    }
}
