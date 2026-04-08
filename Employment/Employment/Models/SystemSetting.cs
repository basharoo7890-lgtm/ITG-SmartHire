using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Models
{
    [Table("System_Setting")]
    public class SystemSetting
    {
        [Key]
        [Column("Setting_Key")]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Column("Setting_value")]
        public string? SettingValue { get; set; }

        public string? Description { get; set; }
    }
}