using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public partial class FontProcessingViewModel : ViewModelBase
    {
        [RelayCommand]
        private void Test()
        {
            var fontList = new List<string>();
            var families = SKFontManager.Default.FontFamilies;
            foreach (var family in families)
            {
                fontList.Add(family);
            }
            //对字体进行排序
            fontList.Sort();
        }
    }
}
