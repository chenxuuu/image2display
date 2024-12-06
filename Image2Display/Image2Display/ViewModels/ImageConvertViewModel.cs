using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Helpers;
using Image2Display.Models;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public partial class ImageConvertViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Bitmap? _rawImage = null;
        [ObservableProperty]
        private int _imageColorCount = 0;

        public bool[] PixelWay
        {
            get => [PixelTraversalOrder == 0, PixelTraversalOrder == 1, PixelTraversalOrder == 2, PixelTraversalOrder == 3,
                        PixelTraversalOrder == 4, PixelTraversalOrder == 5, PixelTraversalOrder == 6, PixelTraversalOrder == 7];
        }

        public int[] IsByteShow
        {
            get
            {
                if (ColorMode == 0)
                {
                    return FullColorStorage switch
                    {
                        0 => [25, 25, 0],
                        <= 3 => [25, 0, 0],
                        <= 6 => [25, 25, 0],
                        <= 8 => [25, 25, 25],
                        9 => [0, 0, 0],
                        _ => throw new NotImplementedException(),
                    };
                }
                else
                {
                    return ColorDepth switch
                    {
                        <= 3 => [0, 0, 0],
                        4 => [25, 0, 0],
                        _ => throw new NotImplementedException(),
                    };
                }
            }
        }

        public string[] ByteContent => Helpers.ByteOrder.GetOrderChars(
            ColorMode,ColorDepth,FullColorStorage,ByteOrder,ColorInternalOrder);
        public SolidColorBrush[] ByteColor => Helpers.ByteOrder.GetOrderColors(
            ColorMode, FullColorStorage, ByteOrder);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPaletteMode), nameof(IsByteShow), 
            nameof(ByteContent), nameof(ByteColor),nameof(IsByteOrderShow))]
        private int _colorMode = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsByteShow), nameof(ByteContent), nameof(ByteColor),
            nameof(IsByteOrderShow))]
        private int _colorDepth = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsByteShow), nameof(ByteContent), nameof(ByteColor),
            nameof(IsByteOrderShow))]
        private int _fullColorStorage = 2;
        //是调色板模式吗
        public bool IsPaletteMode => ColorMode == 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PixelWay))]
        private int _pixelTraversalOrder = 0;
        //字节序有意义吗
        public bool IsByteOrderShow => 
            (ColorMode == 1 && (ColorDepth == 4)) ||
            (ColorMode == 0 && FullColorStorage != 0);
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsByteShow), nameof(ByteContent), nameof(ByteColor))]
        private int _byteOrder = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsByteShow), nameof(ByteContent), nameof(ByteColor))]
        private int _colorInternalOrder = 0;


        public ImageConvertViewModel()
        {
            if (Utils.ImageDataTemp != null)
            {
                Image = Utils.ImageDataTemp;
                Utils.ImageDataTemp = null;
                ImportImage();
            }
            Utils.ImportImageAction = (img) =>
            {
                Image?.Dispose();
                Image = img;
                ImportImage();
            };
        }

        [ObservableProperty]
        private bool _isShowSuccess = false;
        [ObservableProperty]
        private bool _isShowFail = false;
        [ObservableProperty]
        private string _failMessage = "";
        [ObservableProperty]
        private bool _isProcessing = false;

        //处理进度
        [ObservableProperty]
        private int _ProgressValue = 0;

        private ImageData? Image = null;
        private List<Rgba32> ImageColors = new List<Rgba32>();


        /// <summary>
        /// 导入图片后，获取信息
        /// </summary>
        private void ImportImage()
        {
            if(Image == null)
            {
                Debug.WriteLine("Image is null");
                return;
            }
            ImageColors = Image.GetColors();
            Debug.WriteLine($"Color Count: {ImageColors.Count}");
            //把颜色按红色排序
            ImageColors.Sort((a, b) => a.R - b.R);
            ImageColorCount = ImageColors.Count;
            using var stream = Image.GetStream();
            RawImage = new Bitmap(stream);
        }

        private void ShowSuccess()
        {
            IsShowFail = false;
            IsShowSuccess = true;
        }
        private void ShowError(string msgZh, string msgEn)
        {
            if (Utils.Settings.Language.Contains("ZH", StringComparison.CurrentCultureIgnoreCase))
                FailMessage = msgZh;
            else
                FailMessage = msgEn;
            IsShowFail = true;
            IsShowSuccess = false;
        }
        private bool CheckColorVilid()
        {
            if (Image == null)
            {
                ShowError("图片为空", "Image is null");
                return false;
            }
            if (ColorMode == 1)
            {
                var maxCount = ColorDepth switch
                {
                    0 => 2,
                    1 => 4,
                    2 => 16,
                    3 => 256,
                    4 => 65536,
                    _ => 0,
                };
                if (ImageColors.Count > maxCount)
                {
                    ShowError("颜色数量超过调色板容量", "Color count is more than palette capacity");
                    return false;
                }
            }
            return true;
        }

        private async Task<List<byte>?> GetExportData()
        {
            IsProcessing = true;
            List<byte>? r = null;
            ProgressValue = 0;
            void cb(int value)
            {
                ProgressValue = value;
            }
            try
            {
                await Task.Run(() =>
                {
                    if(ColorMode == 1)
                    {
                        r = ColorDepth switch
                        {
                            <= 3 => ColorData.Get1_2_4_8BitsImage(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                (int)double.Pow(2, ColorDepth), ImageColors,cb),
                            4 => ColorData.Get16BitsImage(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, ImageColors, cb),
                            _ => throw new NotImplementedException(),
                        };
                    }
                    else
                    {
                        r = FullColorStorage switch
                        {
                            0 => ColorData.GetRGB444Image(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1, cb),
                            1 => ColorData.GetRGB444HighEmptyImage(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            2 => ColorData.GetRGB565Image(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            3 => ColorData.GetRGB555HighEmptyImage(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            4 => ColorData.GetRGB666HighEmptyImage(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            5 => ColorData.GetRGB666LowEmptyImage(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            6 => ColorData.GetRGB888Image(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            7 => ColorData.GetARGB8888Image(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                            8 => ColorData.GetRGBA8888Image(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1,
                                ByteOrder == 1, cb),
                        9 => ColorData.GetGray8Image(
                                Image!.Raw, PixelTraversalOrder, ColorInternalOrder == 1, cb),
                            _ => throw new NotImplementedException(),
                        };
                    }
                });
            }
            catch(Exception e)
            {
                ShowError("导出失败" + e.Message, "Export failed" + e.Message);
            }
            ProgressValue = 0;
            IsProcessing = false;
            return r;
        }

        private async Task<string> GetExportString(List<byte> data)
        {
            string r = "";
            await Task.Run(() =>
            {
                r = ColorData.ListToCArray(data,Image!.Raw,
                    PixelTraversalOrder, ColorInternalOrder == 1, ByteOrder == 1,
                    ColorMode, ColorMode == 0 ? ColorDepth : FullColorStorage);
            });
            return r;
        }

        [RelayCommand]
        private async Task CopyAsArrayData()
        {
            if (!CheckColorVilid())
                return;
            var data = await GetExportData();
            if (data == null)
                return;
            var str = await GetExportString(data);
            if(await Utils.CopyString(str))
            {
                ShowSuccess();
            }
            else
            {
                ShowError("复制失败", "Copy failed");
            }
        }

        public static FilePickerFileType CFiles { get; } = new("C Files")
        {
            Patterns = ["*.c"],
            AppleUniformTypeIdentifiers = ["public.source-code"],
            MimeTypes = ["text/x-csrc"]
        };
        [RelayCommand]
        private async Task ExportAsArrayFile()
        {
            if (!CheckColorVilid())
                return;
            //保存数据
            var path = await DialogHelper.ShowSaveFileDialogAsync("image_data", CFiles);
            if (path == null)
                return;
            var data = await GetExportData();
            if (data == null)
                return;
            var str = await GetExportString(data);
            try
            {
                await File.WriteAllTextAsync(path, str);
                ShowSuccess();
            }
            catch (Exception e)
            {
                ShowError("保存失败" + e.Message, "Save failed" + e.Message);
            }
        }

        public static FilePickerFileType BinaryFiles { get; } = new("Binary Files")
        {
            Patterns = ["*.bin"],
            AppleUniformTypeIdentifiers = ["public.data"],
            MimeTypes = ["application/octet-stream"]
        };
        [RelayCommand]
        private async Task ExportBinaryFile()
        {
            if (!CheckColorVilid())
                return;
            //保存数据
            var path = await DialogHelper.ShowSaveFileDialogAsync("image_data", BinaryFiles);
            if (path == null)
                return;
            var data = await GetExportData();
            if (data == null)
                return;
            try
            {
                await File.WriteAllBytesAsync(path, [.. data]);
                ShowSuccess();
            }
            catch (Exception e)
            {
                ShowError("保存失败" + e.Message, "Save failed" + e.Message);
            }
        }

        [RelayCommand]
        private async Task ExportColors()
        {
            if (!CheckColorVilid())
                return;
            //保存数据
            var path = await DialogHelper.ShowSaveFileDialogAsync("image_colors", CFiles);
            if (path == null)
                return;
            try
            {
                await File.WriteAllTextAsync(path, ColorData.ColorListToCArray(ImageColors));
                ShowSuccess();
            }
            catch (Exception e)
            {
                ShowError("保存失败" + e.Message, "Save failed" + e.Message);
            }
        }


        [RelayCommand]
        private void Test()
        {
            if (Image == null)
            {
                Debug.WriteLine("Image is null");
                return;
            }
            Debug.WriteLine($"{Image.Width},{Image.Height}");
        }
    }
}
