using Microsoft.UI.Xaml;

namespace TeachersToolbox.App.Helpers;

public static class DpiHelper
{
    /// <summary>
    /// 获取适合当前DPI的字体大小
    /// </summary>
    public static double GetScaledFontSize(double baseFontSize, double scaleFactor)
    {
        return baseFontSize * Math.Min(scaleFactor, 2.0); // 限制最大缩放为2倍
    }

    /// <summary>
    /// 获取建议的边距（基于DPI）
    /// </summary>
    public static Thickness GetScaledThickness(double baseThickness, double scaleFactor)
    {
        var scale = Math.Min(scaleFactor, 2.0);
        return new Thickness(baseThickness * scale);
    }

    /// <summary>
    /// 检查是否为高分辨率显示
    /// </summary>
    public static bool IsHighResolution(double scaleFactor)
    {
        return scaleFactor > 1.5;
    }

    /// <summary>
    /// 获取基于窗口宽度的响应式列数
    /// </summary>
    public static int GetResponsiveColumnCount(double windowWidth)
    {
        if (windowWidth < 640)
            return 1;
        else if (windowWidth < 1024)
            return 2;
        else if (windowWidth < 1440)
            return 3;
        else
            return 4;
    }

    /// <summary>
    /// 获取响应式字体大小
    /// </summary>
    public static double GetResponsiveFontSize(double windowWidth, string sizeCategory = "normal")
    {
        var baseSize = sizeCategory switch
        {
            "small" => 12,
            "normal" => 14,
            "large" => 16,
            "title" => 24,
            "heading" => 28,
            _ => 14
        };

        // 根据窗口宽度调整
        if (windowWidth < 640)
            return baseSize * 0.9;
        else if (windowWidth > 1440)
            return baseSize * 1.1;
        
        return baseSize;
    }
}
