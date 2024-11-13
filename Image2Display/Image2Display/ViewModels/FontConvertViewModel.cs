using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public partial class FontConvertViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Bitmap? _originalImage = null;

        [ObservableProperty]
        private bool _UseSystemFont = true;

        [ObservableProperty]
        private List<ComboBoxItem> _SystemFontList = new();
        [ObservableProperty]
        private int _SelectedFontIndex = 0;
        

        [ObservableProperty]
        private string _FontName = "-----.ttf";

        [ObservableProperty]
        private bool _CharsetUppercaseLetters = false;
        [ObservableProperty]
        private bool _CharsetLowercaseLetters = false;
        [ObservableProperty]
        private bool _CharsetNumbers = false;
        [ObservableProperty]
        private bool _CharsetPunctuation = false;
        [ObservableProperty]
        private bool _CharsetFirstLevelChineseCharacters = false;
        [ObservableProperty]
        private bool _CharsetSecondLevelChineseCharacters = false;
        [ObservableProperty]
        private bool _CharsetGB2312 = false;
        [ObservableProperty]
        private bool _CharsetGB18030 = false;

        [ObservableProperty]
        private string _CustomCharset = "";

        //字体配置
        [ObservableProperty]
        private int _FontSize = 12;
        [ObservableProperty]
        private bool _FontBold = false;
        [ObservableProperty]
        private bool _FontItalic = false;
        [ObservableProperty]
        private bool _FontUnderline = false;
        [ObservableProperty]
        private bool _FontStrikeout = false;

        //字模尺寸
        [ObservableProperty]
        private int _FontWidth = 20;
        [ObservableProperty]
        private int _FontHeight = 20;
        [ObservableProperty]
        private int _FontXOffset = 0;
        [ObservableProperty]
        private int _FontYOffset = 0;

        //灰度或二值化
        [ObservableProperty]
        private bool _UseThreshold = true;
        [ObservableProperty]
        private int _Threshold = 128;
        //灰度位数 0~2: 2、4、8
        [ObservableProperty]
        private int _GrayBitIndex = 0;

        //取模数据处理
        [ObservableProperty]
        private bool _Invert = false;
        [ObservableProperty]
        private int _ByteOrderIndex = 0;
        [ObservableProperty]
        private bool _BitOrderMSB = true;

        //压缩
        [ObservableProperty]
        private bool _RLECompress = false;


        [ObservableProperty]
        private string _PreviewText = Utils.GetI18n<string>("PreviewText");

        public FontConvertViewModel()
        {
            //TODO 初始化图片成格子图
            var pic = new ImageData(100,100);
            pic.AddBackGround(100,100,new SixLabors.ImageSharp.PixelFormats.Rgba32(0,0,0));
            OriginalImage = new Bitmap(pic.GetStream());

            //用skia接口获取系统字体列表
            var fontMgr = SkiaSharp.SKFontManager.Default;
            var fonts = new List<string>();
            foreach (var f in fontMgr.FontFamilies)
            {
                fonts.Add(f);
            }
            //把列表内容按字母排序
            fonts.Sort();
            //添加到系统字体列表
            foreach (var f in fonts)
            {
                var item = new ComboBoxItem
                {
                    Content = f,
                    Tag = f,
                    FontFamily = f
                };
                SystemFontList.Add(item);
            }



        }


        [RelayCommand]
        private async Task ShowPreviousCharacter()
        {
            //TODO 显示上一个字符
        }
        [RelayCommand]
        private async Task ShowNextCharacter()
        {
            //TODO 显示下一个字符
        }
        [RelayCommand]
        private async Task ShowRandomCharacter()
        {
            //TODO 随机显示一个字符
        }

        [RelayCommand]
        private async Task LoadFontFile()
        {
            //TODO 加载字体文件
        }

        [RelayCommand]
        private async Task CopyFontCode()
        {
            //TODO 复制字体代码
        }
        [RelayCommand]
        private async Task SaveFontFile()
        {
            //TODO 保存字体文件
        }
    }
}
