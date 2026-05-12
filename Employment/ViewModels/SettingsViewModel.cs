using System.ComponentModel.DataAnnotations;

namespace Employment.ViewModels
{
    public class SettingsViewModel
    {
        public List<SettingItemViewModel> Settings { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    public class SettingItemViewModel
    {
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsWeight { get; set; }
    }
}