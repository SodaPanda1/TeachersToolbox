namespace TeachersToolbox.Core.Models;

public class AppSettings
{
    public string Theme { get; set; } = "System";
    public string Language { get; set; } = "zh-CN";
    public decimal ExcellentScore { get; set; } = 90;
    public decimal GoodScore { get; set; } = 80;
    public decimal PassScore { get; set; } = 60;
    public bool EnableSounds { get; set; } = true;
    public string DataBackupPath { get; set; } = string.Empty;
    public bool AutoBackup { get; set; } = false;
    public int AutoBackupIntervalDays { get; set; } = 7;
}
