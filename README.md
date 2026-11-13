# Aria2-CSharp

把 aria2 的 RPC 接口封装成一组强类型方法 + 事件回调，用 C# 直接驱动 aria2 的下载任务。

> **状态**：类库 `Aria2c` 可用；`Aria2-CSharp` 这个 WPF 程序目前只是验证接口用的演示壳（一个可拖动的图标 + 数据绑定试验），还不是成品下载器界面。

## 组成部分

| 项目 | 目标框架 | 说明 |
|---|---|---|
| `Aria2c` | .NET Framework 4.5.2 | 类库，主角。aria2 的 RPC 封装：任务增删改查、暂停/恢复、全局统计、事件回调 |
| `Aria2-CSharp` | .NET Framework 4.7.2 | WPF 演示程序 |
| `XmlRpcNet` | .NET Framework 2.0 | 内置的 CookComputing.XmlRpc（XML-RPC 协议实现），供 `Aria2c` 使用 |

## 它能做什么

`Aria2c` 通过 `XmlRpcProxyGen.Create<Aria2cProxy>()` 连接 aria2 的 `http://host:port/rpc`：

- **添加任务**：`AddUri` / `AddTorrent` / `AddMetalink`
- **控制**：`Pause` / `UnPause` / `Remove`（以及对应的 `Force*` 变体）、`PauseAll` / `UnPauseAll`、`ChangePosition`（配合 `PosType` 调整队列顺序）、`ChangeUri`
- **查询**：`TellStatus` / `TellActive` / `TellWaiting` / `TellStopped`、`GetUris` / `GetFiles` / `GetPeers` / `GetServers`、`GetGlobalStat`、`GetVersion`、`GetSessionInfo`
- **选项**：`GetOption` / `ChangeOption`（单任务）、`GetGlobalOption` / `ChangeGlobalOption`（全局）
- **收尾**：`PurgeDownloadResult` / `RemoveDownloadResult` / `SaveSession` / `Shutdown` / `ForceShutdown`
- **事件**：`OnStart`、`OnPaused`、`OnUnPause`、`OnFinish`、`OnAllFinish`、`OnRemoved`、`OnLoading`、`OnError`、`OnWaiting`、`OnGlobalStatusChanged`。内部用定时器轮询任务状态，只在状态变化时触发回调
- **进程托管**：`Aria2cRuntime.Load(settings)` 可自动拉起 `aria2c.exe`

## 快速开始

先让 aria2 以 RPC 模式跑起来（或者交给下面代码里的 `Aria2cRuntime` 自动拉起）：

```bat
aria2c.exe --enable-rpc --rpc-listen-all=true --rpc-allow-origin-all --rpc-listen-port=6800
```

```csharp
using FlyVR.Aria2;

// 1. 连接 aria2；Aria2Path 指向的文件存在时会自动启动进程
Aria2cRuntime.Load(new Aria2cSettings
{
    Aria2Host = "localhost",
    Aria2Port = 6800,
    Aria2Path = @"D:\aria2\aria2c.exe",
    Aria2Args = ""   // 留空则使用内置默认参数
});

if (!Aria2cRuntime.IsLoaded)
{
    Console.WriteLine("aria2 没连上，检查路径和端口");
    return;
}

// 2. 创建下载器并挂事件
using (var aria2 = new Aria2c())
{
    aria2.OnFinish += (s, e) => Console.WriteLine("完成: " + e.Gid);
    aria2.OnError  += (s, e) => Console.WriteLine("失败: " + e.Task.Gid);

    // 3. 添加任务（返回 gid）
    string gid = aria2.AddUri("https://example.com/file.iso", "file.iso", @"D:\Downloads");

    // 4. 查询状态
    Aria2cTask task = aria2.TellStatus(gid);
    Console.WriteLine(task.Status);          // Aria2cTaskStatus：Active / Waiting / Paused / ...

    // 5. 全局速度（byte/s）
    Aria2cGlobalStat stat = aria2.GetGlobalStat();
    Console.WriteLine(stat.DownloadSpeed);
}
```

## 已知问题

- **解决方案无法直接编译**：`Aria2c/Aria2c.csproj` 里有一行 `<ProjectReference Include="..\DebugLog\DebugLog.csproj">`，但仓库里没有 `DebugLog` 目录（`Aria2cRuntime.cs` 里也有 `using Log;`）。要编译得补上该工程，或删掉这条引用并处理 `using Log;`。
- **`Aria2cWarpper` 是 `internal`**：外部调用请走 `Aria2c` / `Aria2cRuntime` 这两个公开入口。
- **默认启动参数写死了端口**：`Aria2cRuntime` 的默认参数是 `--rpc-listen-port=6800`。如果改了 `Aria2Port`，需要在 `Aria2Args` 里自己带上对应端口，否则 RPC 连不上。
- **`Aria2cGlobalStat` 遇到未知字段会抛异常**（`无法将属性转换到Aria2cGlobalStat中`）。aria2 新版本给全局统计加字段时会直接报错，建议改成忽略未知键。
- **目标框架是 .NET Framework**（4.5.2 / 4.7.2），不是 .NET 6+，只能在 Windows 上跑。

## 第三方组件

- `XmlRpcNet/` —— CookComputing.XmlRpc，Copyright (c) 2001-2011, Charles Cook，MIT 许可，源文件内保留原始版权声明
- `Aria2c/` —— aria2 的 C# RPC 封装，部分文件头部标注「创建者：isRight（2016.12.29）」

本项目其余部分以 MIT 许可发布，见 [LICENSE](LICENSE)。
