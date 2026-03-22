# 教师工具箱

一款专为教师设计的课堂教学辅助工具，基于 WinUI 3 构建。

## 功能特性

### 学生管理
- 学生档案管理
- 随机点名（支持滚动动画）
- 随机分组

### 成绩管理
- 成绩录入与管理
- 成绩统计分析
- 成绩排名

### 课堂工具
- 课堂计时器（正计时/倒计时）
- 随机抽题
- 加减分板

### 作业管理
- 作业布置
- 完成情况统计

### 行政工具
- 课程表管理
- 考勤管理
- 通知生成

## 技术栈

- .NET 8.0 LTS
- WinUI 3 (Windows App SDK)
- SQLite + Dapper
- CommunityToolkit.Mvvm
- ClosedXML (Excel导出)

## 运行要求

- Windows 10 版本 1809 (17763) 或更高
- Windows App SDK 1.6 运行时

## 运行方式

### 方式一：使用启动脚本
```
run.bat
```

### 方式二：命令行运行
```bash
dotnet run --project src/TeachersToolbox.App/TeachersToolbox.App.csproj
```

### 方式三：直接运行可执行文件
```
src\TeachersToolbox.App\bin\Debug\net8.0-windows10.0.19041.0\win-x64\TeachersToolbox.App.exe
```

## 如果遇到运行时错误

如果提示 "Windows App Runtime" 相关错误，请安装 Microsoft Windows App SDK 运行时：

1. 访问 https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads
2. 下载并安装对应版本的运行时安装程序
3. 重新运行应用

## 项目结构

```
TeachersToolbox/
├── src/
│   ├── TeachersToolbox.Core/    # 核心业务层（跨平台）
│   ├── TeachersToolbox.Data/    # 数据访问层
│   └── TeachersToolbox.App/     # WinUI 3 应用层
├── tests/                       # 单元测试
└── TeachersToolbox.sln          # 解决方案文件
```

## 开发说明

### 构建项目
```bash
dotnet build TeachersToolbox.sln
```

### 运行测试
```bash
dotnet test
```

### 跨平台迁移
Core 和 Data 层使用 .NET 8.0，可在 Linux 上运行。
UI 层可使用 Avalonia UI 进行跨平台迁移。
