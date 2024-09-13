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

        [ObservableProperty]
        private bool[] _pixelWay = [true, false, false, false, false, false, false, false];
        [ObservableProperty]
        private bool[] _isByteShow = [true, false, false];
        [ObservableProperty]
        private string[] _byteContent = [
            "-", "-", "-", "-", "-", "-", "-", "-",
            "-", "-", "-", "-", "-", "-", "-", "-",
            "-", "-", "-", "-", "-", "-", "-", "-",
            "-", "-", "-", "-", "-", "-", "-", "-",
            ];
        private static readonly SolidColorBrush GrayColor = new (new Color(0x7f, 0x7f, 0x7f, 0x7f));
        [ObservableProperty]
        private Brush[] _byteColor = [
            GrayColor,GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor,
            GrayColor,GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor,
            GrayColor,GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor,
            GrayColor,GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor, GrayColor,
            ];

        [ObservableProperty]
        private int _colorMode = 0;
        [ObservableProperty]
        private int _colorDepth = 0;
        [ObservableProperty]
        private int _fullColorStorage = 2;

        [ObservableProperty]
        private int _pixelTraversalOrder = 0;
        [ObservableProperty]
        private int _byteOrder = 0;
        [ObservableProperty]
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
            ImageColorCount = ImageColors.Count;
            using var stream = Image.GetStream();
            RawImage = new Bitmap(stream);
        }



        [RelayCommand]
        private async Task CopyAsArrayData()
        {

        }
        [RelayCommand]
        private async Task ExportAsArrayFile()
        {

        }
        [RelayCommand]
        private async Task ExportBinaryFile()
        {

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
