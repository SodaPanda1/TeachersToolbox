@echo off
echo ========================================
echo 教师工具箱 启动器
echo ========================================
echo.

echo 正在启动教师工具箱...
echo.

start "" "src\TeachersToolbox.App\bin\Debug\net8.0-windows10.0.19041.0\win-x64\TeachersToolbox.App.exe"

if %ERRORLEVEL% EQU 0 (
    echo 应用程序已启动!
) else (
    echo 启动失败，请检查错误信息。
    echo.
    echo 如果提示 "Windows App Runtime" 相关错误，请确保安装了
    echo Microsoft Windows App SDK 1.6 运行时:
    echo https://aka.ms/windowsappsdk/1.6/latest/windowsappruntimeinstall-x64.exe
)

pause
