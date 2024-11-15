using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Helpers;
using Image2Display.Models;
using SkiaSharp;
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
        private bool _EnableUseSystemFontCheckBox = true;

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
        private bool _CharsetChinesePunctuation = false;
        [ObservableProperty]
        private bool _CharsetFirstLevelChineseCharacters = false;
        [ObservableProperty]
        private bool _CharsetSecondLevelChineseCharacters = false;
        [ObservableProperty]
        private bool _CharsetGB2312 = false;

        [ObservableProperty]
        private string _CustomCharset = "";

        [ObservableProperty]
        private int _TotalCharacters = 0;

        //字体配置
        [ObservableProperty]
        private int _FontSize = 50;
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
        private int _FontWidth = 50;
        [ObservableProperty]
        private int _FontHeight = 50;
        [ObservableProperty]
        private int _FontXOffset = 0;
        [ObservableProperty]
        private int _FontYOffset = 0;

        //灰度或二值化
        [ObservableProperty]
        private bool _UseThreshold = true;
        [ObservableProperty]
        private byte _Threshold = 128;
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

        //提示文本
        [ObservableProperty]
        private bool _IsShowSuccess = false;
        [ObservableProperty]
        private bool _IsShowFail = false;
        [ObservableProperty]
        private string _FailMessage = "";

        private void ShowSuccess()
        {
            IsShowSuccess = true;
            IsShowFail = false;
        }
        private void ShowFail(string message)
        {
            IsShowSuccess = false;
            IsShowFail = true;
            FailMessage = message;
        }

        [ObservableProperty]
        private string _PreviewText = Utils.GetI18n<string>("PreviewText");

        private ImageData? ImageCache = null;


        /// <summary>
        /// FontConvertViewModel初始化
        /// </summary>
        public FontConvertViewModel()
        {
            //初始化图片成格子图
            var pic = FontConvert.GetImage(DemoFontData.Default, 20, 20, ImageCache);
            using var ps = pic.GetStream();
            OriginalImage = new Bitmap(ps);

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
            if(SystemFontList.Count > 0)
                ReloadSKTypeface();
            else
            {
                UseSystemFont = false;
                EnableUseSystemFontCheckBox = false;
            }
            /*****************************************/
            //字符集需要刷新的情况
            string[] charsetChangedElements =
            [
                    nameof(CharsetUppercaseLetters),
                    nameof(CharsetLowercaseLetters),
                    nameof(CharsetNumbers),
                    nameof(CharsetPunctuation),
                    nameof(CharsetChinesePunctuation),
                    nameof(CharsetFirstLevelChineseCharacters),
                    nameof(CharsetSecondLevelChineseCharacters),
                    nameof(CharsetGB2312),
                    nameof(CustomCharset)
            ];
            //字符预览需要刷新的情况
            string[] previewChangedElements =
            [
                    nameof(UseSystemFont),
                    nameof(SelectedFontIndex),
                    nameof(FontName),
                    nameof(FontSize),
                    nameof(FontBold),
                    nameof(FontItalic),
                    nameof(FontUnderline),
                    nameof(FontStrikeout),
                    nameof(FontWidth),
                    nameof(FontHeight),
                    nameof(FontXOffset),
                    nameof(FontYOffset),
                    nameof(UseThreshold),
                    nameof(Threshold),
                    nameof(GrayBitIndex),
                    nameof(Invert),
            ];
            //字体需要刷新的情况
            string[] fontChangedElements =
            [
                    nameof(UseSystemFont),
                    nameof(SelectedFontIndex),
                    nameof(FontBold),
                    nameof(FontItalic),
                    nameof(FontUnderline),
                    nameof(FontStrikeout),
            ];

            PropertyChanged += async (sender, e) =>
            {
                //某个变量被更改
                var name = e.PropertyName;

                //更新字体
                if (fontChangedElements.Contains(name))
                    ReloadSKTypeface();
                //刷新字符集
                if (previewChangedElements.Contains(name))
                    await RefreshPreview();
                //刷新预览
                if (charsetChangedElements.Contains(name))
                {
                    await RefreshCharset();
                    await RefreshPreview();
                }
            };

        }


        private List<char> chars = [];
        private async Task RefreshCharset()
        {
            await Task.Run(() =>
            {
                chars.Clear();

                if (!string.IsNullOrEmpty(CustomCharset))
                    chars.AddRange(CustomCharset);
                //去除回车换行
                chars.RemoveAll(c => c == '\r' || c == '\n');

                if (CharsetUppercaseLetters)
                    chars.AddRange(Charset.GetUppercaseLetters());
                if (CharsetLowercaseLetters)
                    chars.AddRange(Charset.GetLowercaseLetters());
                if (CharsetNumbers)
                    chars.AddRange(Charset.GetNumbers());
                if (CharsetPunctuation)
                    chars.AddRange(Charset.GetPunctuation());
                if (CharsetChinesePunctuation)
                    chars.AddRange(Charset.GetChinesePunctuation());
                if (CharsetFirstLevelChineseCharacters)
                    chars.AddRange(Charset.GetFirstLevelChinese());
                if (CharsetSecondLevelChineseCharacters)
                    chars.AddRange(Charset.GetSecondLevelChinese());
                if (CharsetGB2312)
                    chars.AddRange(Charset.GetGB2312());

                //去除重复字符
                chars = chars.Distinct().ToList();
                //刷新字符数量
                TotalCharacters = chars.Count;
                //预览索引重置
                PreviewIndex = 0;
            });
        }

        private int PreviewIndex = 0;
        private async Task RefreshPreview()
        {
            await Task.Run(() =>
            {
                //如果没有字符，显示默认字符
                if (chars.Count == 0)
                {
                    var pic = FontConvert.GetImage(DemoFontData.Default, 20, 20, ImageCache);
                    using var ps = pic.GetStream();
                    OriginalImage = new Bitmap(ps);
                    return;
                }

                if (LoadedFont == null)
                    return;

                //获取原始图像数据
                var data = FontConvert.GetData(LoadedFont,
                    FontSize, chars[PreviewIndex], 
                    FontWidth, FontHeight, 
                    FontXOffset, FontYOffset);

                // 各种处理
                if (UseThreshold)
                {
                    FontConvert.ThresholdImage(data, Threshold);
                }
                else
                {
                    var grayBit = GrayBitIndex switch
                    {
                        0 => 2,
                        1 => 4,
                        2 => 8,
                        _ => 2
                    };
                    FontConvert.GrayScaleImage(data, grayBit);
                }
                if(Invert)
                    FontConvert.InvertImage(data);

                var fp = FontConvert.GetImage(data, FontWidth, FontHeight, ImageCache);
                using var fps = fp.GetStream();
                OriginalImage = new Bitmap(fps);
            });
        }

        [RelayCommand]
        private async Task ShowPreviousCharacter()
        {
            if (chars.Count == 0)
                return;
            PreviewIndex -= 1;
            if (PreviewIndex < 0)
                PreviewIndex = chars.Count - 1;
            await RefreshPreview();
        }
        [RelayCommand]
        private async Task ShowNextCharacter()
        {
            if (chars.Count == 0)
                return;
            PreviewIndex += 1;
            if (PreviewIndex >= chars.Count)
                PreviewIndex = 0;
            await RefreshPreview();
        }
        [RelayCommand]
        private async Task ShowRandomCharacter()
        {
            if (chars.Count == 0)
                return;
            PreviewIndex = new Random().Next(0, chars.Count);
            await RefreshPreview();
        }

        private SKTypeface? LoadedFont = null;
        private string? LastFontPath = null;
        [RelayCommand]
        private async Task LoadFontFile()
        {
            var fileType = new FilePickerFileType("Font")
            {
                Patterns = ["*.ttf","*.otf"],
                AppleUniformTypeIdentifiers = ["public.item"],
                MimeTypes = ["font/ttf", "font/otf"]
            };
            var files = await DialogHelper.ShowOpenFileDialogAsync(fileType, false);
            if (files.Count == 0)
                return;
            var filePath = files[0].Path.LocalPath;

            //尝试加载字体文件，失败则提示
            if(!ReloadSKTypeface(filePath))
            {
                ShowFail(Utils.GetI18n<string>("LoadFontFileFail"));
                return;
            }

            FontName = Path.GetFileName(filePath);
            LastFontPath = filePath;
            await RefreshPreview();
        }

        private bool ReloadSKTypeface(string? path = null)
        {
            if (UseSystemFont)
            {
                if (SystemFontList.Count == 0)
                    return false;
                var fontName = SystemFontList[SelectedFontIndex].Tag as string;
                LoadedFont = SKTypeface.FromFamilyName(fontName,
                    FontBold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                    SKFontStyleWidth.Normal,
                    FontItalic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
            }
            else
            {
                path ??= LastFontPath;
                if (string.IsNullOrEmpty(path))
                    return false;
                var font = SKTypeface.FromFile(path);
                if (font == null)
                    return false;
                LoadedFont = font;
            }
            return true;
        }


        private async Task<string> GetFontData()
        {
            if(LoadedFont == null)
            {
                ShowFail("no font");
                return string.Empty;
            }
            var result = string.Empty;
            var error = string.Empty;
            await Task.Run(() =>
            {
                var data = new List<List<byte>>();
                foreach (var c in chars)
                {
                    var raw = FontConvert.GetData(LoadedFont,
                        FontSize, c,
                        FontWidth, FontHeight,
                        FontXOffset, FontYOffset);

                    var grayBit = GrayBitIndex switch
                    {
                        0 => 2,
                        1 => 4,
                        2 => 8,
                        _ => 2
                    };
                    var d = FontConvert.GetResultData(raw, FontWidth, FontHeight,
                        !UseThreshold, grayBit, Threshold,
                        Invert, ByteOrderIndex, BitOrderMSB);
                    data.Add(d);
                }
                result = FontConvert.ByteListToCArray(data, FontWidth, FontHeight, chars);
            });
            if (!string.IsNullOrEmpty(error))
            {
                ShowFail(error);
                return string.Empty;
            }
            return result;
        }

        [RelayCommand]
        private async Task CopyFontCode()
        {
            var data = await GetFontData();
            if(string.IsNullOrEmpty(data))
                return;
            if (await Utils.CopyString(data))
            {
                ShowSuccess();
            }
            else
            {
                ShowFail("Copy failed");
            }
        }
        public static FilePickerFileType CFiles { get; } = new("C Files")
        {
            Patterns = ["*.c"],
            AppleUniformTypeIdentifiers = ["public.source-code"],
            MimeTypes = ["text/x-csrc"]
        };
        [RelayCommand]
        private async Task SaveFontFile()
        {
            //保存数据
            var path = await DialogHelper.ShowSaveFileDialogAsync("font_data", CFiles);
            if (path == null)
                return;
            var data = await GetFontData();
            if (string.IsNullOrEmpty(data))
                return;
            try
            {
                await File.WriteAllTextAsync(path, data);
                ShowSuccess();
            }
            catch (Exception e)
            {
                ShowFail("Save failed: " + e.Message);
            }
        }
    }
}
