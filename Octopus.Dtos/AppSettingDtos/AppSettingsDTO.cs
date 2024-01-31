namespace Octopus.Dtos.AppSettingDtos
{
    public class AppSettingsDTO
    {
        public string[] TextsToSearch { get; set; }
        public AppSettingsConfigurations Configurations { get; set; }
        public Device DeviceConfigurations { get; set; }
    }

    public class AppSettingsConfigurations
    {
        public bool IsInsuranceTypeCustomerEnabled { get; set; }
        public bool IsDoctorsDeskScreenEnabled { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public bool IsOrderTabSectionsAutoCollapseEnabled { get; set; }
    }
    public class Device
    {
        public string ConfigurationFileName { get; set; }
        public string CurrentDeviceResolution { get; set; }
    }
}
