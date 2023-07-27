using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="language">语言代码，如zh-cn</param>
        private static void ChangeLanguage(string language)
        {
            var file = $"avares://Image2Display/Assets/Languages/{language}.axaml";
            if (File.Exists(language))
                file = language;
            var data =
                new ResourceInclude(new Uri(file, UriKind.Absolute));
            data.Source = new Uri(file, UriKind.Absolute);
            Application.Current!.Resources.MergedDictionaries[0] = data;
        }
    }
}
