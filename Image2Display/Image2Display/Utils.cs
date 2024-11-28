using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using HarfBuzzSharp;
using Image2Display.Helpers;
using Image2Display.Models;
using Microsoft.Extensions.Configuration;
using Sentry;
using Sentry.Profiling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Image2Display;

public class Utils
{
    private static string? _version = null;
    /// <summary>
    /// 软件版本
    /// </summary>
    public static string Version
    {
        get
        {
            if (_version is null)
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
                _version = $"{version.Major}.{version.Minor}.{version.Build}";
            }
            return _version;
        }
    }

    /// <summary>
    /// 用户配置文件路径
    /// win：C:\Users\{username}\Image2Display
    /// linux：/home/{username}/Image2Display
    /// mac：/Users/{username}/Image2Display
    /// </summary>
    public readonly static string AppPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Image2Display");
    /// <summary>
    /// 配置文件路径
    /// </summary>
    private static readonly string _settingsPath = Path.Combine(AppPath, "settings.json");
    private static Models.SettingModel? _settings = null;
    /// <summary>
    /// 软件配置
    /// </summary>
    public static Models.SettingModel Settings
    {
        get
        {
            if (_settings is null)//初始化
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile(_settingsPath, true, false);
                var config = builder.Build();
                _settings = config.Get<Models.SettingModel>() ?? new Models.SettingModel();
            }
            return _settings!;
        }
    }

    /// <summary>
    /// 启动软件时的初始化操作
    /// </summary>
    public static void Initial()
    {
        if (!Directory.Exists(AppPath))
            Directory.CreateDirectory(AppPath);
        //初始化语言
        ChangeLanguage(Settings.Language);
        //恢复设置的主题色
        Application.Current!.RequestedThemeVariant = Utils.Settings.Theme switch
        {
            1 => ThemeVariant.Light,
            2 => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }

    /// <summary>
    /// 设置语言
    /// </summary>
    /// <param name="language">语言代码，如zh-cn</param>
    public static void ChangeLanguage(string language = "en-US", bool test = false)
    {
        var file = $"avares://Image2Display/Assets/Languages/{language}.axaml";
        if (test)
            file = $"avares://I2D_Test/{language}.axaml";
        var data =
            new ResourceInclude(new Uri(file, UriKind.Absolute))
            {
                Source = new Uri(file, UriKind.Absolute)
            };
        try
        {
            Application.Current!.Resources.MergedDictionaries[0] = data;
        }
        catch
        {
            ChangeLanguage();
        }
    }

    /// <summary>
    /// 主动保存配置
    /// </summary>
    public static void SaveSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings, SourceGenerationContext.Default.SettingModel);
        File.WriteAllText(_settingsPath, jsonString);
    }

    /// <summary>
    /// 获取当前语言的实际数据
    /// </summary>
    /// <param name="name">数据名称</param>
    /// <returns>数据</returns>
    public static T GetI18n<T>(string name)
    {
        try
        {
            Application.Current!.TryFindResource(name, out object? value);
            return (T)value!;
        }
        catch
        {
            if (typeof(T) == typeof(string))
                return (T)(object)"??";
            else
                return default!;
        }
    }

    /// <summary>
    /// 打开网页链接
    /// </summary>
    /// <param name="url">网址</param>
    public static async Task OpenWebLink(string url)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch
        {
            await DialogHelper.ShowUnableToOpenLinkDialog(new Uri(url));
        }
    }

    /// <summary>
    /// 主页面切换到目标选项卡
    /// </summary>
    /// <param name="target"></param>
    public static Action<int>? SwitchPage { get; set; }

    /// <summary>
    /// 中转图片数据
    /// </summary>
    public static ImageData? ImageDataTemp { get; set; }
    /// <summary>
    /// 主动传递图片数据的方法，用于跨页面传递
    /// </summary>
    public static Action<ImageData>? ImportImageAction { get; set; }

    public static void SetImage2ConvertPage(ImageData img)
    {
        if (ImportImageAction is null)//如果没有设置传递方法，则说明还没初始化
            ImageDataTemp = img;
        else//直接传递
            ImportImageAction(img);
    }

    public static async Task<bool> CopyString(string txt)
    {
        try
        {
            TopLevel top;
            var app = Application.Current!.ApplicationLifetime;
            if (app is IClassicDesktopStyleApplicationLifetime desktop)
            {
                top = desktop.MainWindow!;
            }
            else if (app is ISingleViewApplicationLifetime single)
            {
                top = TopLevel.GetTopLevel(single.MainView)!;
            }
            else
                return false;
            var clipboard = top.Clipboard;
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, txt);
            await clipboard!.SetDataObjectAsync(dataObject);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void InitSentry()
    {
        SentrySdk.Init(options =>
        {
            // A Sentry Data Source Name (DSN) is required.
            // See https://docs.sentry.io/product/sentry-basics/dsn-explainer/
            // You can set it in the SENTRY_DSN environment variable, or you can set it in code here.
            options.Dsn = "https://584c6a99c6245ce35ee09b9c2d09f467@o4508334826651648.ingest.de.sentry.io/4508374769926224";

            // When debug is enabled, the Sentry client will emit detailed debugging information to the console.
            // This might be helpful, or might interfere with the normal operation of your application.
            // We enable it here for demonstration purposes when first trying Sentry.
            // You shouldn't do this in your applications unless you're troubleshooting issues with Sentry.
#if DEBUG
            options.Debug = true;
#endif

            // This option is recommended. It enables Sentry's "Release Health" feature.
            options.AutoSessionTracking = true;

            // Set TracesSampleRate to 1.0 to capture 100%
            // of transactions for tracing.
            // We recommend adjusting this value in production.
            options.TracesSampleRate = 1.0;

            // Sample rate for profiling, applied on top of othe TracesSampleRate,
            // e.g. 0.2 means we want to profile 20 % of the captured transactions.
            // We recommend adjusting this value in production.
            options.ProfilesSampleRate = 1.0;
            // Requires NuGet package: Sentry.Profiling
            // Note: By default, the profiler is initialized asynchronously. This can
            // be tuned by passing a desired initialization timeout to the constructor.
            options.AddIntegration(new ProfilingIntegration(
                // During startup, wait up to 500ms to profile the app startup code.
                // This could make launching the app a bit slower so comment it out if you
                // prefer profiling to start asynchronously
                TimeSpan.FromMilliseconds(500)
            ));
        });
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            SentrySdk.CaptureException((Exception)args.ExceptionObject);
        };
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            SentrySdk.CaptureException(args.Exception);
            args.SetObserved(); // 防止应用程序崩溃
        };
    }
}
