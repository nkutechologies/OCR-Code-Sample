using Octopus.Dtos.AppSettingDtos;

namespace Octopus.Helpers.CustomHelpers
{
    public static class AppSettingManager
    {
        public static void Save(string key, string value)
        {
            Properties.Settings.Default[key] = value;
            Properties.Settings.Default.Save();
        }
        public static void Reset(string key)
        {
            Properties.Settings.Default[key] = string.Empty;
            Properties.Settings.Default.Save();
        }
        //read from appsettings.json
        public static (int Width, int Height) GetCurrentResolution(AppSettingsDTO appSettings)
        {
            string resolution = appSettings?.DeviceConfigurations?.CurrentDeviceResolution;

            if (!string.IsNullOrEmpty(resolution))
            {
                string[] dimensions = resolution.Split('x');

                if (dimensions.Length == 2 && int.TryParse(dimensions[0], out int width) && int.TryParse(dimensions[1], out int height))
                {
                    return (width, height);
                }
            }

            // Default value if resolution is not set or cannot be parsed
            return (0, 0);
        }
        //read from app property settings
        //public static (int Width, int Height) GetCurrentResolution()
        //{
        //    string resolution = Properties.Settings.Default.CurrentDeviceResolution;

        //    if (!string.IsNullOrEmpty(resolution))
        //    {
        //        string[] dimensions = resolution.Split('x');

        //        if (dimensions.Length == 2 && int.TryParse(dimensions[0], out int width) && int.TryParse(dimensions[1], out int height))
        //        {
        //            return (width, height);
        //        }
        //    }

        //    // Default value if resolution is not set or cannot be parsed
        //    return (0, 0);
        //}
        ////save in app property settings
        //public static void SaveUpdatedResolution(string key, int width, int height)
        //{
        //    string resolution = $"{width}x{height}";
        //    Properties.Settings.Default[key] = resolution;
        //    Properties.Settings.Default.Save();
        //}
    }
}
