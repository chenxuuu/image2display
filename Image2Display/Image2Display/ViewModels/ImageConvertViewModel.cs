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
                        <= 5 => [25, 25, 0],
                        <= 7 => [25, 25, 25],
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
