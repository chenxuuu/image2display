using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using HarfBuzzSharp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Image2Display
{
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

        private static string _settingsPath = "settings.json";
        public static Models.SettingModel? _settings = null;
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
            //初始化语言
            ChangeLanguage(Settings.Language);
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
                new ResourceInclude(new Uri(file, UriKind.Absolute));
            data.Source = new Uri(file, UriKind.Absolute);
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
            string jsonString = JsonSerializer.Serialize(Settings);
            File.WriteAllText(_settingsPath, jsonString);
        }

        /// <summary>
        /// 打开网页链接
        /// </summary>
        /// <param name="url">网址</param>
        public static void OpenWebLink(string url)
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
            catch { }
        }
    }
}
