using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.Models
{
    public class SettingModel
    {
        private static string[] _supportLanguages = new string[] { "zh-CN", "en-US" };
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
                    if(!_supportLanguages.Contains(lang))//不支持的语言一律用英语
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
    }
}
