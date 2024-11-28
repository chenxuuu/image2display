using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Image2Display.Models
{
    [JsonSerializable(typeof(SettingModel))]
    [JsonSourceGenerationOptions(WriteIndented = true)]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }

    public class SettingModel
    {
        public static readonly string[] SupportLanguages = ["zh-CN", "en-US"];
        private string? _language = null;
        /// <summary>
        /// 软件语言
        /// </summary>
        public string Language
        {
            get
            {
                if (_language is null)
                {
                    var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                    if(!SupportLanguages.Contains(lang))//不支持的语言一律用英语
                        _language = "en-US";
                    else
                        _language = lang;
                }
                return _language!;
            }
            set
            {
                _language = value;
            }
        }

        /// <summary>
        /// 软件主题色
        /// 0:跟随系统
        /// 1:浅色
        /// 2:深色
        /// </summary>
        public int Theme { get; set; } = 0;
    }
}
